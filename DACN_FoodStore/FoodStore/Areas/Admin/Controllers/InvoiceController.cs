using FoodStore.Models;
using FoodStore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodStore.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class InvoiceController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ITableRepository _tableRepository;
        private readonly IQRCodeRepository _qrcodeRepository;
        private readonly ApplicationDbContext _context;
        public InvoiceController(IOrderRepository orderRepository, IInvoiceRepository invoiceRepository, ITableRepository tableRepository, IQRCodeRepository qrcodeRepository,ApplicationDbContext context )
        {
            _orderRepository = orderRepository;
            _invoiceRepository = invoiceRepository;
            _tableRepository = tableRepository;
            _qrcodeRepository = qrcodeRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceRepository.GetAllAsync();
            ViewBag.invoices = invoices;
            return View(invoices);
        }

        [HttpGet]
        public async Task<IActionResult> BillPaid()
        {
            var invoices = await _invoiceRepository.GetAllAsync();
            ViewBag.invoices = invoices;
            return View(invoices);
        }
        [HttpGet]
        public async Task<IActionResult> GetListOrderAccept()
        {
            var order = await _orderRepository.GetListOrderAccept();
            return View(order);
        }
        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            var order = await _orderRepository.GetOrderAcceptedById(id);
            var orderDetails = await _orderRepository.GetListOrderDetailsByIdOrder(id);
            ViewBag.OrderDetail = orderDetails;
            if (order == null)
            {
                return NotFound();
            }
            ViewBag.Id = order.Id;
            ViewBag.idTable = order.TableId;
            ViewBag.order = order;
            return View();
        }

        public async Task<IActionResult> ListInvoice()
        {
            var list = await _invoiceRepository.GetAllPaidAsync();

            return View(list);
        }
        [HttpPost]
        public async Task<IActionResult> CompleteInvoice(int OrderId, long changeValue, int paymentId)
        {

            Invoice invoice = new Invoice();
            var order = await _orderRepository.GetOrderAcceptedById(OrderId);
            var table = await _tableRepository.GetByIdAsync(order.TableId);

            if (changeValue > 0)
            {
                invoice.Price = order.TotalPrice + changeValue;
            }
            invoice.PaymentId = paymentId;
            invoice.Price = order.TotalPrice + changeValue;
            invoice.OrderId = order.Id;
            invoice.CreatedAt = DateTime.UtcNow;
            invoice.FinishedAt = DateTime.UtcNow;

            var result = await _invoiceRepository.AddAsync(invoice);

            if (result != null)
            {
                table.Status = 0;
                _context.Tables.Update(table);
                await _context.SaveChangesAsync();
                return Redirect("/admin/Invoice");
            }
            else
            {
                return NotFound();
            }

        }

        [HttpGet]
        public async Task<IActionResult> DetailUnpaid(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            var order = await _orderRepository.GetOrderAcceptedById(invoice.OrderId);
            if (order == null)
            {
                return NotFound();
            }
            var orderDetail = await _orderRepository.GetListOrderDetailsByIdOrder(order.Id);
            ViewBag.Order = order;
            ViewBag.OrderDetail = orderDetail;
            ViewBag.Invoice = invoice;
            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> DetailPaid(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            var order = await _orderRepository.GetOrderAcceptedById(invoice.OrderId);
            if (order == null)
            {
                return NotFound();
            }
            var orderDetail = await _orderRepository.GetListOrderDetailsByIdOrder(order.Id);
            ViewBag.Order = order;
            ViewBag.OrderDetail = orderDetail;
            ViewBag.Invoice = invoice;
            return View(order);
        }

        [HttpPost]
        [Route("admin/invoice/complete/{id:int}")]

        public async Task<IActionResult> Complete(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);

            var table = await _tableRepository.GetByIdAsync(invoice.Order.TableId);
            var qrcode = await _qrcodeRepository.GetQRCodeByIdTableAsync(invoice.Order.TableId);
            if (invoice == null)
            {
                return NotFound();
            }
            table.Status = 0;
            qrcode.Status = true;
            invoice.Status = true;
            await _tableRepository.UpdateAsync(table);
            await _qrcodeRepository.UpdateQRCodeAsync(qrcode);
            await _invoiceRepository.UpdateAsync(invoice, table);
            return RedirectToAction("Index");
        }

        //mới thêm Status và Charge (update database)
        /* public async Task<IActionResult> CompleteInvoice(Invoice invoice)
         {
             if (ModelState.IsValid)
             {
                 var order = await _orderRepository.GetOrderAcceptedById(invoice.OrderId);
                 await _orderRepository.UpdatePayAsync(invoice.OrderId);
                 invoice.Status = true;
                 if (invoice.Charge > 0)
                 {
                     invoice.Price = order.TotalPrice + invoice.Charge;
                 }
                 invoice.CreatedAt = DateTime.UtcNow;
                 await _tableRepository.UpdateStatus(order.TableId);
                 var result = await _invoiceRepository.CreateAsync(invoice);

                 if (result != null)
                 {

                     return Redirect("/owner/ManagementInvoice/ListInvoice");
                 }
                 else
                 {
                     return NotFound();
                 }
             }
             return View(invoice);
         }*/
        //[HttpGet]
        /* public async Task<IActionResult> ListInvoice()
         {
             var userId = int.Parse(User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value);
             var listStore = await _storeRepository.GetStoresByUserId(userId);
             storeId = listStore.Select(x => x.Id).FirstOrDefault();
             var list = await _invoiceRepository.GetAllPaidAsync(storeId);

             return View(list);
         }*/
        //[HttpGet]
        /*public async Task<IActionResult> InvoiceDetails(int id)
        {
            try
            {
                var invoice = await _invoiceRepository.GetInvoicePaidById(id);

                if (invoice == null)
                {
                    return NotFound();
                }

                var order = await _orderRepository.GetOrderPaidAsync(invoice.Order.TableId);

                if (order == null)
                {
                    return NotFound();
                }

                var orderDetail = await _orderRepository.GetListOrderDetailsByIdOrder(order.Id);

                ViewBag.Order = order;
                ViewBag.OrderDetail = orderDetail;

                return View(invoice);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }*/


    }
}
