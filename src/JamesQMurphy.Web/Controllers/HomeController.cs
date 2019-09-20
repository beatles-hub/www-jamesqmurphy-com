﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using JamesQMurphy.Blog;
using JamesQMurphy.Web.Models;

namespace JamesQMurphy.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IArticleStore articleStore;
        private readonly WebSiteOptions webSiteOptions;
        private readonly UserManager<ApplicationUser> userManager;

        public HomeController(IArticleStore iarticleStore, WebSiteOptions webSiteOptions, UserManager<ApplicationUser> userMgr)
        {
            articleStore = iarticleStore;
            this.webSiteOptions = webSiteOptions;
            userManager = userMgr;
        }

        public IActionResult Index()
        {
            // Need to optimize this
            var lastTwoArticles = articleStore.GetArticles().TakeLast(2).ToList();
            var article1 = articleStore.GetArticle(lastTwoArticles[1].YearString, lastTwoArticles[1].MonthString, lastTwoArticles[1].Slug);
            var article2 = articleStore.GetArticle(lastTwoArticles[0].YearString, lastTwoArticles[0].MonthString, lastTwoArticles[0].Slug);
            var homePageItems = new HomePageItems(article1, article2);

            return View(homePageItems);
        }

        public IActionResult About()
        {
            ViewData[Constants.VIEWDATA_PAGETITLE] = "About This Site";
            ViewData[Constants.VIEWDATA_MARKDOWN] = System.IO.File.ReadAllText("Views/Home/About.md");
            return View("Article");
        }

        public IActionResult Privacy()
        {
            ViewData[Constants.VIEWDATA_PAGETITLE] = "Privacy Policy";
            ViewData[Constants.VIEWDATA_MARKDOWN] = System.IO.File.ReadAllText("Views/Home/Privacy.md").Replace("@webSiteTitle", webSiteOptions.WebSiteTitle);
            return View("Article");
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Secret()
        {
            ViewData[Constants.VIEWDATA_PAGETITLE] = "Secret Page";
            ViewData["User"] = await userManager.GetUserAsync(this.User);
            return View("Secret");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
