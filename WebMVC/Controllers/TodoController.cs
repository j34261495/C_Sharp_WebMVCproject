using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMVC.Models;

namespace WebMVC.Controllers
{
    public class NewsController : Controller
    {
        private readonly WebContext _context;

        public NewsController(WebContext context)
        {
            _context = context;
        }

        // 1️⃣ 顯示所有新聞
        public async Task<IActionResult> Index()
        {
            var newsList = await _context.Test.ToListAsync();
            return View(newsList);
        }

        // 2️⃣ 顯示「新增新聞」的表單
        public IActionResult Create()
        {
            return View();
        }

        // 3️⃣ 接收表單資料並新增新聞
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(News news)
        {
            if (ModelState.IsValid)
            {
                _context.Add(news);
                await _context.SaveChangesAsync();  // 🔥 非同步寫入資料庫
                return RedirectToAction(nameof(Index));  // 成功後回首頁
            }
            return View(news);  // 若驗證失敗，回到表單
        }
    }
}
