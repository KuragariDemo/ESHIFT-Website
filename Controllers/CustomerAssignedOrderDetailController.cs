using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using EShift.Data;
using EShift.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

namespace EShift.Controllers
{
    public class CustomerAssignedOrderDetailController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerAssignedOrderDetailController(ApplicationDbContext context)
        {
            _context = context;
        }

        // View page
        public IActionResult CustomerAssignedOrderDetail(int id)
        {
            var data = GetData(id);
            return View("~/Views/CustomerDashboard/CustomerAssignedOrderDetail.cshtml", data);
        }

        // Export to Excel
        public IActionResult ExportExcel(int id)
        {
            var data = GetData(id);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("AssignedOrders");

            worksheet.Cell(1, 1).Value = "Order ID";
            worksheet.Cell(1, 2).Value = "From Address";
            worksheet.Cell(1, 3).Value = "To Address";
            worksheet.Cell(1, 4).Value = "Customer ID";
            worksheet.Cell(1, 5).Value = "Customer Name";
            worksheet.Cell(1, 6).Value = "Phone";
            worksheet.Cell(1, 7).Value = "Car ID";
            worksheet.Cell(1, 8).Value = "Car License No";
            worksheet.Cell(1, 9).Value = "Car Model";
            worksheet.Cell(1, 10).Value = "Car Type";
            worksheet.Cell(1, 11).Value = "Driver Name";
            worksheet.Cell(1, 12).Value = "Assistant ID";
            worksheet.Cell(1, 13).Value = "Assistant Name";
            worksheet.Cell(1, 14).Value = "Scheduled Date Time";

            int row = 2;
            foreach (var item in data)
            {
                worksheet.Cell(row, 1).Value = item.OrderId;
                worksheet.Cell(row, 2).Value = item.FromAddress;
                worksheet.Cell(row, 3).Value = item.ToAddress;
                worksheet.Cell(row, 4).Value = item.UserId;
                worksheet.Cell(row, 5).Value = item.CustomerName;
                worksheet.Cell(row, 6).Value = item.Phone;
                worksheet.Cell(row, 7).Value = item.CarId;
                worksheet.Cell(row, 8).Value = item.CarLicNo;
                worksheet.Cell(row, 9).Value = item.CarModel;
                worksheet.Cell(row, 10).Value = item.CarType;
                worksheet.Cell(row, 11).Value = item.DriverName;
                worksheet.Cell(row, 12).Value = item.AssistantId;
                worksheet.Cell(row, 13).Value = item.AssistantName;
                worksheet.Cell(row, 14).Value = item.ScheduledDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AssignedOrders.xlsx");
        }

        // Export to PDF
        public IActionResult ExportPDF(int id)
        {
            var data = GetData(id);

            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            document.Add(new Paragraph("Customer Assigned Orders")
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetFontSize(16));

            foreach (var item in data)
            {
                document.Add(new Paragraph($"Order ID: {item.OrderId}"));
                document.Add(new Paragraph($"From: {item.FromAddress}"));
                document.Add(new Paragraph($"To: {item.ToAddress}"));
                document.Add(new Paragraph($"Customer: {item.CustomerName} (ID: {item.UserId})"));
                document.Add(new Paragraph($"Phone: {item.Phone}"));
                document.Add(new Paragraph($"Car: {item.CarLicNo} - {item.CarModel} ({item.CarType})"));
                document.Add(new Paragraph($"Driver: {item.DriverName}"));
                document.Add(new Paragraph($"Assistant: {item.AssistantName} (ID: {item.AssistantId})"));
                document.Add(new Paragraph($"Scheduled Date/Time: {item.ScheduledDateTime:yyyy-MM-dd HH:mm:ss}"));
                document.Add(new Paragraph("-----------------------------"));
            }

            document.Close();
            return File(stream.ToArray(), "application/pdf", "AssignedOrders.pdf");
        }

        // Private method to get assigned orders for customer
        private List<CustomerAssignedOrderDetailViewModel> GetData(int customerId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return new List<CustomerAssignedOrderDetailViewModel>(); // or handle as unauthorized
            }

            var assignedJobs = _context.AssignedJobs
                .Include(j => j.Order)
                .Where(j => j.UserId == userId)
                .ToList();

            var cars = _context.Cars.ToList();
            var assistants = _context.Assistants.ToList();

            var result = new List<CustomerAssignedOrderDetailViewModel>();

            foreach (var job in assignedJobs)
            {
                var order = job.Order;
                if (order == null) continue;

                var car = cars.FirstOrDefault(c => c.CarID == job.CarId);
                var driver = assistants.FirstOrDefault(a => a.AssistantID == job.DriverId);
                var loaderId = job.LoaderIds?.Split(',').FirstOrDefault();
                var loader = assistants.FirstOrDefault(a => a.AssistantID.ToString() == loaderId);

                result.Add(new CustomerAssignedOrderDetailViewModel
                {
                    OrderId = order.OrderId,
                    FromAddress = order.FromAddress,
                    ToAddress = order.ToAddress,
                    UserId = order.UserId,
                    CustomerName = order.Name,
                    Phone = order.PhoneNumber,
                    CarId = car?.CarID ?? 0,
                    CarLicNo = car?.CarLicenseNo ?? "N/A",
                    CarModel = car?.CarModel ?? "N/A",
                    CarType = car?.CarType ?? "N/A",
                    DriverName = driver?.AssistantName ?? "N/A",
                    AssistantId = loader?.AssistantID ?? 0,
                    AssistantName = loader?.AssistantName ?? "N/A",
                    ScheduledDateTime = job.ScheduledDateTime,
                    Status = job.Status
                });
            }
            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinishOrder(int orderId)
        {
            var customerOrder = await _context.CustomerOrders.FindAsync(orderId);
            if (customerOrder == null)
            {
                return NotFound(); // If order not found
            }

            // Step 1: Change status to Finished on the customer side
            customerOrder.Status = "Finished"; // Mark as finished
            _context.CustomerOrders.Update(customerOrder);

            // Step 2: Update assigned jobs related to the order
            var assignedJobs = _context.AssignedJobs.Where(j => j.OrderId == orderId).ToList();
            foreach (var job in assignedJobs)
            {
                // Mark car as available
                var car = await _context.Cars.FindAsync(job.CarId);
                if (car != null)
                {
                    car.Status = "Available";
                    car.AssignedJobId = null;
                    _context.Cars.Update(car);
                }

                // Mark driver as available
                var driver = await _context.Assistants.FindAsync(job.DriverId);
                if (driver != null)
                {
                    driver.Status = "Available";
                    driver.AssignedJobId = null;
                    _context.Assistants.Update(driver);
                }

                // Mark loaders as available
                var loaderIds = job.LoaderIds?.Split(',') ?? Array.Empty<string>();
                foreach (var loaderId in loaderIds)
                {
                    if (int.TryParse(loaderId, out var id))
                    {
                        var loader = await _context.Assistants.FindAsync(id);
                        if (loader != null)
                        {
                            loader.Status = "Available";
                            loader.AssignedJobId = null;
                            _context.Assistants.Update(loader);
                        }
                    }
                }
            }

            // Step 3: Save all changes in the database
            await _context.SaveChangesAsync();

            // Redirect back to the customer's order list page
            return RedirectToAction("CusOrderList", "CustomerOrder");
        }

    }
}
