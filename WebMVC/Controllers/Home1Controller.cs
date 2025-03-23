using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMVC.Models;

namespace WebMVC.Controllers
{
    public class Home1Controller : Controller
    {
        private readonly WebContext _kcgContext;  // 資料庫物件

        // 依賴注入，將 WebContext 傳入建構子
        public Home1Controller(WebContext kcgContext)
        {
            _kcgContext = kcgContext;
        }

        // 1️ 顯示資料庫中的所有 Test 資料
        public async Task<IActionResult> Index()
        {
            var tests = await _kcgContext.Test.ToListAsync();  // 以非同步方式取得資料
            return View(tests);  // 回傳資料到 Index 頁面
        }

        // 2️ 顯示「新增 Test」的表單
        public IActionResult Create()
        {
            return View();
        }

        // 3️ 處理新增資料的表單提交
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Test test)
        {
            if (ModelState.IsValid)
            {
                // 檢查是否有空餘的 Id
                var existingIds = await _kcgContext.Test.Select(t => t.Id).ToListAsync(); // 獲取所有已存在的 Id
                int newId = 1; // 假設最小的 ID 是 1

                // 查找空缺的 Id
                for (int i = 1; i <= existingIds.Count; i++)
                {
                    if (!existingIds.Contains(i))
                    {
                        newId = i;
                        break;
                    }
                }

                // 如果沒有找到空缺的 Id，則設為現有最大 Id + 1
                if (newId == 1 && !existingIds.Contains(1))
                {
                    newId = 1;
                }
                else if (newId == 1)
                {
                    newId = existingIds.Max() + 1;
                }

                // 設定新的 Id
                test.Id = newId;

                // 新增資料
                _kcgContext.Add(test);
                await _kcgContext.SaveChangesAsync();  // 非同步儲存

                return RedirectToAction(nameof(Index));  // 新增成功後，回到 Index 頁面
            }

            return View(test);  // 若資料無效，返回 Create 頁面
        }


        // 4️ 顯示資料的詳細頁面
        public async Task<IActionResult> Details(int id)
        {
            var test = await _kcgContext.Test.FirstOrDefaultAsync(t => t.Id == id);  // 查詢單一資料
            if (test == null)
            {
                return NotFound();  // 若找不到資料，回傳 404
            }
            return View(test);  // 回傳資料到 Details 頁面
        }

        // 5️ 顯示編輯頁面
        public async Task<IActionResult> Edit(int id)
        {
            var test = await _kcgContext.Test.FindAsync(id);  // 查詢單一資料
            if (test == null)
            {
                return NotFound();  // 若找不到資料，回傳 404
            }
            return View(test);  // 返回資料到 Edit 頁面
        }

        // 6️ 處理編輯頁面的資料更新
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Test test)
        {
            if (id != test.Id)
            {
                return NotFound();  // 如果 ID 不符合，回傳 404
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _kcgContext.Update(test);  // 更新資料
                    await _kcgContext.SaveChangesAsync();  // 非同步儲存
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_kcgContext.Test.Any(t => t.Id == id))
                    {
                        return NotFound();  // 若資料不存在，回傳 404
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));  // 更新成功後，回到 Index 頁面
            }
            return View(test);  // 若資料無效，返回 Edit 頁面
        }

        // 7️ 顯示刪除頁面
        public async Task<IActionResult> Delete(int id)
        {
            var test = await _kcgContext.Test.FirstOrDefaultAsync(t => t.Id == id);  // 查詢資料
            if (test == null)
            {
                return NotFound();  // 若資料不存在，回傳 404
            }
            return View(test);  // 返回資料到 Delete 頁面
        }

        // 8️ 處理刪除資料的表單提交
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            var test = await _kcgContext.Test.FindAsync(id);

            if (test == null)
            {
                // 找不到數據時，返回更友好的錯誤頁面或重定向
                return RedirectToAction(nameof(Index));  // 或者顯示錯誤消息
            }

            try
            {
                _kcgContext.Test.Remove(test);
                await _kcgContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // 記錄錯誤
                Console.WriteLine(ex.Message);
                // 返回錯誤視圖
                return View("Error");
            }
        }
    }
}
