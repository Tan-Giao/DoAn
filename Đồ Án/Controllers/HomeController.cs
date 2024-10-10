using Assignment.Data;
using Assignment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using BCrypt.Net;
using System.Linq;
using Azure.Core;
using System.Timers;
using System.Threading;
using Assignment.Data.Entities;
using Assignment.Controllers;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Versioning;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
namespace Assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private UserManager<Account> _userManager;
        private SignInManager<Account> _signInManager;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<Account> userManager, SignInManager<Account> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        private async Task<List<Product>> Get12Products(int tab)
        {
            var skipCount = tab * 12 - 12;
            return await _context.Products.Include(l => l.Category).Include(l => l.Brand).Skip(skipCount).Take(12).ToListAsync();
        }

        private async Task<int> GetTotalProducts()
        {
            return await _context.Products.CountAsync();
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Get4Products = await _context.Products.Take(4).ToArrayAsync();
            ViewBag.GetBestSeller = await _context.Products.Take(8).ToArrayAsync();
            ViewBag.GetLaptop = await _context.Products.Include(l => l.Category).Where(p => p.Category.CategoryName == "Laptop").Take(8).ToArrayAsync();
            ViewBag.GetMobile = await _context.Products.Include(l => l.Category).Where(p => p.Category.CategoryName == "Smart Phone").Take(8).ToArrayAsync();
            ViewBag.BestProduct = await _context.Products.FindAsync(29);
            return View(await _context.Products.Take(12).ToArrayAsync());
        }


        //public IActionResult Login([Bind("UserName, Password")] Account ac)
        //{
        //    //WaitHandle wait = new AutoResetEvent(false);
        //    //wait.WaitOne(2000);
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }
        //    var user = _context.Accounts.SingleOrDefault(a => a.UserName == ac.UserName);
        //    if (user!=null)
        //    {
        //        if(BCrypt.Net.BCrypt.Verify(ac.Password, user.PasswordHash))
        //        {

        //            return RedirectToAction("Index");
        //        }
        //    }
        //    return View();
        //}

        #region Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserInput model)
        {
            //WaitHandle wait = new AutoResetEvent(false);
            //wait.WaitOne(2000);
            if (!ModelState.IsValid || model.EmailOrUserName == null)
            {
                return View();
            }
            Account? user = null;
            if (IsEmailValid(model.EmailOrUserName))
                user = await _context.Accounts.SingleOrDefaultAsync(a => a.Email == model.EmailOrUserName);
            else
                user = await _context.Accounts.SingleOrDefaultAsync(a => a.UserName == model.EmailOrUserName);

            if (user != null && user.UserName != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        #endregion


        #region Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserInput model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = _context.Accounts.SingleOrDefaultAsync(a => model.UserName == a.UserName || model.Email == a.Email);
            if (user.Result != null)
            {
                throw new Exception("User already exists");
            }
            var result = await _userManager.CreateAsync(model, model.Password);

            if (result.Succeeded && CreateTokenEmailAsync(model) == null)
            {
                await _context.SaveChangesAsync();
                return RedirectToAction("WaitingConfirmationEmail");
            }
            return View();

        }

        #endregion


        public async Task<IActionResult> AllProduct(int tab /*, SearchModel search*/)
        {
            ViewBag.Get4Products = await _context.Products.Take(4).ToArrayAsync();

            //if (search == null)
                //return View(await _context.Products
                //    .Include(p => p.Category)
                //    .Include(p => p.Brand)
                //    .ToArrayAsync());


            //var product = _context.Products
            //    .Include(p => p.Brand)
            //    .Include(p => p.Category)
            //    .Where(p => p.ProductId.ToString() == search.ToString() || p.ProductName.Contains(search.ToString()) || p.Brand.BrandName.Contains(search.ToString()) || p.Category.CategoryName.Contains(search.ToString()))
            //    .ToList();

            //var viewModel = new SearchModel
            //{
            //    Products = product,
            //    SearchText = search
            //};
            //return View(product);
            if (tab == 0)
                tab = 1;
            ViewData["tab"] = tab;
            ViewData["total"] = await GetTotalProducts();
            return View(await Get12Products(tab));
         
        }


        public IActionResult ProductDetail(int id)
        {
            var product = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .SingleOrDefault(p => p.ProductId == id);
            ViewBag.ViewMore = _context.Products
                .OrderBy(x => Guid.NewGuid())
                .Take(4)
                .ToArray();
            return View(product);
        }

        //public IActionResult Search(string search)
        //{
        //    return RedirectToAction("AllProduct", new {product});
        //}


        #region Confirm Email Method
        private bool IsEmailValid(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public void SendEmail(string email, string subject, string htmlMessage)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("kiet43012@gmail.com", "fjrq yuus fmaf ugbt"),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("kiet43012@gmail.com"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            smtpClient.Send(mailMessage);

        }
        public IActionResult WaitingConfirmationEmail()
        {
            ViewData["Email"] = HttpContext.Session.GetString("ConfirmEmail");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                // Xử lý lỗi khi xác nhận email không thành công
                return RedirectToAction("Error");
            }
        }

        public async Task<string?> CreateTokenEmailAsync(UserInput model)
        {
            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(model);
                var confirmationLink = Url.Action("ConfirmEmail", "Home", new { userId = model.Id, token }, Request.Scheme);
                var emailSubject = "Xác nhận tài khoản của bạn";
                var emailBody = $"Nhấp vào liên kết sau để xác nhận tài khoản của bạn: <a href='{confirmationLink}'>Xác nhận</a>";
                if (model.Email != null)
                {
                    SendEmail(model.Email, emailSubject, emailBody);
                    HttpContext.Session.SetString("ConfirmEmail", model.Email);
                    return null;
                }
                else
                    throw new Exception("Email is null");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        #endregion

        #region Cart Method

        public IActionResult Buy(int id)
        {
            var product = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .SingleOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return RedirectToAction("Cart");
            }


            if (SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart") == null)
            {
                List<Item> cart = new List<Item>();
                cart.Add(new Item { Products = product, Amount = 1 });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
                int index = isExist(id);
                if (index != -1)
                {
                    if (cart[index].Products.ProductPrice != product.ProductPrice)
                    {
                        var tempAmount = cart[index].Amount;
                        cart[index] = (new Item { Products = product, Amount = tempAmount + 1 });
                    }
                    else
                    {
                        cart[index].Amount++;
                    }
                }
                else
                {
                    cart.Add(new Item { Products = product, Amount = 1 });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }

            return RedirectToAction("Cart");
        }

        public IActionResult Cart()
        {
            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            ViewData["Total"] = cart.Sum(item => item.Products.ProductPrice * item.Amount);
            return View(cart);
        }

        private int isExist(int id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count(); i++)
            {
                if (cart[i].Products.ProductId.Equals(id))
                    return i;
            }
            return -1;
        }
        private List<Item> GetCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<Item>>("cart");
            if (cart == null)
            {
                cart = new List<Item>();
            }
            return cart;
        }
        public IActionResult Remove(int id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = isExist(id);
            cart.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Cart");
        }
        public IActionResult Clear()
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            while (cart.Count != 0)
            {
                cart.RemoveAt(0);
            }
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);

            return RedirectToAction("Cart");
        }
        public ActionResult IncreaseAmount(int id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = isExist(id);
            cart[index].Amount++;
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Cart");
        }
        public ActionResult DecreaseAmount(int id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = isExist(id);
            cart[index].Amount--;
            if (cart[index].Amount == 0)
            {
                cart.RemoveAt(index);
            }
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Cart");
        }
        #endregion


        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
