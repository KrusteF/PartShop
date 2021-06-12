using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using PartShop.Data;
using PartShop.Models;
using PartShop.Models.ViewModels;
using PartShop.Utility;

namespace PartShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly int PageSize = 1500;
        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderHeaderOrderDetailsViewModel OrderVM = new()
            {
                OrderHeader = await _db.OrderHeader.Include(p => p.ApplicationUser).FirstOrDefaultAsync(p => p.Id == id && p.UserId == claim.Value),
                OrderDetails = await _db.OrderDetails.Where(p => p.OrderId == id).ToListAsync()
            };

            return View(OrderVM);
        }

        [Authorize]
        public async Task<IActionResult> OrderHistory(int productPage = 1)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<OrderHeader> orderHeaderList = await _db.OrderHeader.Include(p => p.ApplicationUser).Where(p => p.UserId == claim.Value).ToListAsync();

            OrderListViewModel orderListVM = new()
            {
                Orders = new List<OrderHeaderOrderDetailsViewModel>()
            };

            foreach (OrderHeader item in orderHeaderList)
            {
                OrderHeaderOrderDetailsViewModel individual = new()
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(p => p.OrderId == item.Id).ToListAsync()
                };
                orderListVM.Orders.Add(individual);
            }

            var count = orderListVM.Orders.Count;
            orderListVM.Orders = orderListVM.Orders
                                    .OrderByDescending(p => p.OrderHeader.Id)
                                     .Skip((productPage - 1) * PageSize)
                                      .Take(PageSize).ToList();

            orderListVM.PagingInfo = new PagingInfo()
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItem = count,
                UrlParam = "/Customer/Order/OrderHistory?productPage=:"
            };

            return View(orderListVM);
        }

        [Authorize(Roles = SD.AdminUser)]
        public async Task<IActionResult> ManageOrder()
        {
            List<OrderHeaderOrderDetailsViewModel> orderDetailsVM = new();

            List<OrderHeader> orderHeaderList = await _db.OrderHeader.Where(p => p.Status == SD.StatusSubmitted || p.Status == SD.StatusInProcess)
                                                    .OrderByDescending(p => p.PickupTime).ToListAsync();

            foreach (OrderHeader item in orderHeaderList)
            {
                OrderHeaderOrderDetailsViewModel individual = new()
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(p => p.OrderId == item.Id).ToListAsync()
                };
                orderDetailsVM.Add(individual);
            }

            return View(orderDetailsVM.OrderBy(p => p.OrderHeader.PickupTime).ToList());
        }

        public async Task<IActionResult> GetOrderDetails(int id)
        {
            OrderHeaderOrderDetailsViewModel orderVM = new()
            {
                OrderHeader = await _db.OrderHeader.Include(p => p.ApplicationUser).FirstOrDefaultAsync(p => p.Id == id),
                OrderDetails = await _db.OrderDetails.Where(p => p.OrderId == id).ToListAsync()
            };
            return PartialView("_IndividualOrderDetails", orderVM);
        }

        public IActionResult GetOrderStatus(int id)
        {
            return PartialView("_OrderStatus", _db.OrderHeader.Where(p => p.Id == id).FirstOrDefault().Status);
        }

        [Authorize(Roles = SD.AdminUser)]
        public async Task<IActionResult> OrderPrepare(int OrderId)
        {
            OrderHeader orderheaderById = await _db.OrderHeader.FindAsync(OrderId);
            orderheaderById.Status = SD.StatusInProcess;
            await _db.SaveChangesAsync();

            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = SD.AdminUser)]
        public async Task<IActionResult> OrderCancel(int OrderId)
        {
            OrderHeader orderheaderById = await _db.OrderHeader.FindAsync(OrderId);
            orderheaderById.Status = SD.StatusCancelled;
            await _db.SaveChangesAsync();

            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = SD.AdminUser)]
        public async Task<IActionResult> OrderReady(int OrderId)
        {
            OrderHeader orderheaderById = await _db.OrderHeader.FindAsync(OrderId);
            orderheaderById.Status = SD.StatusReady;
            await _db.SaveChangesAsync();

            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize]
        public async Task<IActionResult> OrderPickup(int productPage = 1, string searchName = null, string searchPhone = null, string searchEmail = null)
        {

            StringBuilder param = new();
            param.Append("/Customer/Order/OrderPickup?productPage=:");
            param.Append("&searchName=");
            if (searchName != null)
            {
                param.Append(searchName);
            }
            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }
            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }

            List<OrderHeader> orderHeaderList = new();

            if (searchName != null || searchPhone != null || searchEmail != null)
            {
                var user = new ApplicationUser();
                if (searchName != null)
                {
                    orderHeaderList = await _db.OrderHeader.Include(p => p.ApplicationUser).
                                                Where(p => p.PickupName.ToLower().Contains(searchName.ToLower()))
                                                    .OrderByDescending(p => p.OrderDate).ToListAsync();
                }
                else
                {
                    if (searchPhone != null)
                    {
                        orderHeaderList = await _db.OrderHeader.Include(p => p.ApplicationUser).
                                                    Where(p => p.PickupNumber.Contains(searchPhone))
                                                        .OrderByDescending(p => p.OrderDate).ToListAsync();
                    }
                    else
                    {
                        if (searchEmail != null)
                        {
                            user = await _db.ApplicationUser.Where(p => p.Email.ToLower().Contains(searchEmail.ToLower())).FirstOrDefaultAsync();
                            orderHeaderList = await _db.OrderHeader.Include(p => p.ApplicationUser).
                                                        Where(p => p.UserId == user.Id)
                                                            .OrderByDescending(p => p.OrderDate).ToListAsync();
                        }
                    }
                }
            }
            else
            {
                orderHeaderList = await _db.OrderHeader.Include(p => p.ApplicationUser).Where(p => p.Status == SD.StatusReady).ToListAsync();
            }

            OrderListViewModel orderListVM = new()
            {
                Orders = new List<OrderHeaderOrderDetailsViewModel>()
            };



            foreach (OrderHeader item in orderHeaderList)
            {
                OrderHeaderOrderDetailsViewModel individual = new()
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(p => p.OrderId == item.Id).ToListAsync()
                };
                orderListVM.Orders.Add(individual);
            }

            var count = orderListVM.Orders.Count;
            orderListVM.Orders = orderListVM.Orders
                                    .OrderByDescending(p => p.OrderHeader.Id)
                                     .Skip((productPage - 1) * PageSize)
                                      .Take(PageSize).ToList();

            orderListVM.PagingInfo = new PagingInfo()
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItem = count,
                UrlParam = param.ToString()
            };

            return View(orderListVM);
        }

        [Authorize(Roles = SD.AdminUser)]
        [HttpPost, ActionName("OrderPickup")]
        public async Task<IActionResult> OrderPickupPost(int orderId)
        {
            OrderHeader orderById = await _db.OrderHeader.FindAsync(orderId);
            orderById.Status = SD.StatusCompleted;
            await _db.SaveChangesAsync();
            return RedirectToAction("OrderPickup", "Order");
        }
    }
}