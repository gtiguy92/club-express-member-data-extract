using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

class UAIPMember
{
  public UAIPMember(IWebElement element) {
    _element = element;

    this.ExtractName();
    this.ExtractEmail();
    this.ExtractPhone();
    this.ExtractProfileLink();
  }

  public string Name { get; set; }
  public string Email { get; set; }
  public string Phone { get; set; }
  public string ProfileLink { get; set; }
  
  private readonly IWebElement _element;

  private void ExtractName() {
    this.Name = _element.FindElement(By.ClassName("biz-member-link")).Text;
  }

  private void ExtractEmail() {
    var emailElement = _element.FindElements(By.ClassName("biz-email-link")).SingleOrDefault();

    this.Email = emailElement?.Text;
  }

  private void ExtractPhone() {
    var phoneElement = _element.FindElements(By.ClassName("biz-cell-phone")).SingleOrDefault();

    if (string.IsNullOrWhiteSpace(phoneElement?.Text))
    {
      phoneElement = _element.FindElements(By.ClassName("biz-home-phone")).SingleOrDefault();
    }

    this.Phone = phoneElement?.Text;
  }

  private void ExtractProfileLink() {
    var profileLinkElement = _element.FindElements(By.ClassName("biz-member-link")).SingleOrDefault();

    this.ProfileLink = profileLinkElement?.GetAttribute("href");
  }

  public string ToString(char delimiter) {
    return string.Join(delimiter, new List<string> () {this.Name, this.Email, this.Phone});
  }
}