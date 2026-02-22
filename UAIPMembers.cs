using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

class UAIPMembers {
  
  private string _clubExpressUrl;
  private string _username;
  private string _password;

  // Constructor to initialize the private variables
  public UAIPMembers(string url, string username, string password)
  {
    _clubExpressUrl = url;
    _username = username;
    _password = password;
  }

  public void Test()
  {
    new DriverManager().SetUpDriver(new ChromeConfig());

    ChromeOptions options = new ChromeOptions();

    var driver = new ChromeDriver();

    driver.Navigate().GoToUrl(_clubExpressUrl);

    driver.FindElement(By.ClassName("login-link")).Click();

    driver.FindElement(By.Id("ctl00_ctl00_login_name")).SendKeys(_username);
    driver.FindElement(By.Id("ctl00_ctl00_password")).SendKeys(_password);
    driver.FindElement(By.Id("ctl00_ctl00_login_button")).Click();

    new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(drv => drv.FindElement(By.LinkText("Membership Directory"))).Click();

    new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(drv => drv.FindElement(By.Id("ctl00_ctl00_subgroup_dropdown_tree"))).Click();

    var chapters = new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(drv => drv.FindElements(By.ClassName("rtIn")));

    chapters.Single(elem => elem.Text.Contains("Flint Tribe")).Click();

    // Maximize the window so the search button is not covered
    driver.Manage().Window.Maximize();

    driver.FindElement(By.Id("ctl00_ctl00_search_button")).Click();

    // Wait for the member count to be more than 0
    var memberCount = new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(d => d.FindElements(By.ClassName("biz-info")).Count() > 0);

    List<UAIPMember> members = new List<UAIPMember>();
    List<UAIPMember> membersOnPage;
    UAIPMember firstMemberOnPage = null;
    int pageNumber = 1;

    while (true)
    {
      membersOnPage = driver.FindElements(By.ClassName("biz-info")).Select(mem => new UAIPMember(mem)).ToList();

      Console.WriteLine($"Found {membersOnPage.Count} member(s) on page {pageNumber}.");

      firstMemberOnPage = membersOnPage.First();
      members.AddRange(membersOnPage);

      IWebElement nextButton = driver.FindElement(By.CssSelector("a.next"));
      if (nextButton.GetAttribute("class")?.Contains("disabled") == true)
        break;

      string firstMemberNameBeforeClick = firstMemberOnPage.Name;
      nextButton.Click();

      new WebDriverWait(new SystemClock(), driver, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(5))
        .Until(d =>
        {
          var elements = d.FindElements(By.ClassName("biz-info"));
          if (elements.Count == 0) return false;
          try
          {
            var currentFirstName = elements.First().FindElement(By.ClassName("biz-member-link")).Text;
            return currentFirstName != firstMemberNameBeforeClick;
          }
          catch (StaleElementReferenceException)
          {
            return false;
          }
        });

      pageNumber++;
    }

    Console.WriteLine($"Found {members.Count()} member(s) total.");

    members.ForEach(m => Console.WriteLine(m.ToString('|')));

    Console.WriteLine("Email Addresses");
    Console.WriteLine(String.Join(',', members.Select(m => m.Email)));

    Console.WriteLine("Phone Numbers");
    Console.WriteLine(String.Join(',', members.Select(m => m.Phone)));
  }
}