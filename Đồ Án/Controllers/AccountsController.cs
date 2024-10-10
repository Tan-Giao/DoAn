using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment.Data;
using Assignment.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Assignment.Models;
using System.Data;
using AutoMapper;

namespace Assignment.Controllers
{
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private UserManager<Account> _userManagers;
        private readonly IMapper _mapper;
        public AccountsController(ApplicationDbContext context, UserManager<Account> user, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _userManagers = user;
        }
        private async Task<List<Product>> Get12Products(int tab)
        {
            var skipCount = tab * 12 - 12;
            return await _context.Products.Skip(skipCount).Take(10).ToListAsync();
        }

        private async Task<int> GetTotalProducts()
        {
            return await _context.Accounts.CountAsync();
        }
        // GET: Products
        public async Task<IActionResult> Index(int tab)
        {
            if (tab == 0)
                tab = 1;
            ViewData["tab"] = tab;
            ViewData["total"] = await GetTotalProducts();
            return View(await Get12Products(tab));
        }

        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = Guid.NewGuid();
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserInput account)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManagers.CreateAsync(account,account.Password);
                //account.Id = Guid.NewGuid();
                //_context.Add(account);
                //await _context.SaveChangesAsync();
                if (!result.Succeeded)
                {
                    throw new Exception("Can not create the account.");
                }
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Account input)
        {
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var account = await _userManagers.FindByIdAsync(id.ToString());
            if (account == null)
            {
                return NotFound();
            }

            account.UserName = input.UserName;
            account.Email = input.Email;
            account.PhoneNumber = input.PhoneNumber;
            account.FullName = input.FullName;
            account.Address = input.Address;
            account.Avatar = input.Avatar;
            account.Gender = input.Gender;

            var result = await _userManagers.UpdateAsync(account);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(account.Id))
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

        // GET: Accounts/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(Guid id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
