using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment.Data;
using Assignment.Data.Entities;

namespace Assignment.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public static List<Brand>? _brand;
        public static List<Category>? _category;
        
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
            _brand = GetBrands();
            _category = GetCategories();
        }

        private List<Brand> GetBrands()
        {
            return _context.Brands.ToList(); ;
        }
        private List<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }
        private async Task<List<Product>> Get10Products(int tab)
        {
            var skipCount = tab * 10 - 10;
            return await _context.Products.Skip(skipCount).Take(10).ToListAsync();
        }
        private async Task<int> GetTotalProducts()
        {
            return await _context.Products.CountAsync();
        }
        // GET: Products
        public async Task<IActionResult> Index(int tab)
        {
            if (tab == 0)
                tab = 1;
            ViewData["tab"] = tab;
            ViewData["total"] = await GetTotalProducts();
            return View(await Get10Products(tab));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context
                .Products.Include(p => p.Category).Include(p => p.Brand)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {

            ViewBag.Brands = _brand;
            ViewBag.Categories = _category;
            return View();
        }


        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductQuantity,ProductDescription,ProductPrice,ProductImage,ProductScreen,ProductPlatform,ProductCamera,ProductChip,ProductRam,ProductStorage,ProductBattery,ProductColor,BrandId,CategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.Brands = _brand;
            ViewBag.Categories = _category;
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["Platform"] = product.ProductPlatform;
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductQuantity,ProductDescription,ProductPrice,ProductImage,ProductScreen,ProductPlatform,ProductCamera,ProductChip,ProductRam,ProductStorage,ProductBattery,ProductColor,BrandId,CategoryId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
