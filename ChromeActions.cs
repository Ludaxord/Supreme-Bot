using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Supreme_Bot

{
    public class ChromeActions
    {
        public string pathToChromium =  Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public string chooseSize = null;

        public ChromeActions(string chooseSize) {
            this.chooseSize = chooseSize;
        }

        public void InitChrome(String pageUrl) {
            Console.WriteLine(pathToChromium);
            IWebDriver webDriver = new ChromeDriver(pathToChromium);
            webDriver.Navigate().GoToUrl(url: pageUrl);
            IWebElement buyButton = webDriver.FindElement(By.Name("commit"));
            IWebElement sizeElement = webDriver.FindElement(By.Id("size"));
            if (chooseSize != "Uni") {
                var size = new SelectElement(sizeElement);
                size.SelectByText(chooseSize);
            }
            int milliseconds = 150;
            Thread.Sleep(milliseconds);
            buyButton.Click();
            Thread.Sleep(milliseconds);
            ToCheckout(webDriver);
            Thread.Sleep(milliseconds);
            //TestPayment(webDriver);
            ToPayment(webDriver);
        }

        public void ToCheckout(IWebDriver webDriver) {
            IWebElement checkOutButton = webDriver.FindElement(By.ClassName("checkout"));
            checkOutButton.Click();
        }

        public void TestPayment(IWebDriver webDriver) {
            webDriver.FindElement(By.XPath("//input[@id='order_billing_name' and @name='order[billing_name]']")).SendKeys("<NAME>");
   

        }

        public void ToPayment(IWebDriver webDriver) {
            new WebDriverWait(webDriver, TimeSpan.FromSeconds(3))
                .Until(ExpectedConditions.ElementToBeClickable(By.XPath("//input[@id='order_billing_name' and @name='order[billing_name]']")))
                .SendKeys("<NAME>");

            new WebDriverWait(webDriver, TimeSpan.FromSeconds(3))
                .Until(ExpectedConditions.ElementToBeClickable(By.XPath("//input[@id='order_email' and @name='order[email]']")))
               .SendKeys("<EMAIL>");

            new WebDriverWait(webDriver, TimeSpan.FromSeconds(3))
                .Until(ExpectedConditions.ElementToBeClickable(By.XPath("//input[@id='order_tel' and @name='order[tel]']")))
                .SendKeys("<PHONE_NUMBER>");

            new WebDriverWait(webDriver, TimeSpan.FromSeconds(3))
            .Until(ExpectedConditions.ElementToBeClickable(By.XPath("//input[@id='bo' and @name='order[billing_address]']")))
            .SendKeys("<STREET_NAME>");

            new WebDriverWait(webDriver, TimeSpan.FromSeconds(3))
            .Until(ExpectedConditions.ElementToBeClickable(By.XPath("//input[@id='oba3' and @name='order[billing_address_2]']")))
            .SendKeys("<HOME_NUMBER>");

            new WebDriverWait(webDriver, TimeSpan.FromSeconds(3))
            .Until(ExpectedConditions.ElementToBeClickable(By.XPath("//input[@id='order_billing_city' and @name='order[billing_city]']")))
            .SendKeys("<CITY>");

            new WebDriverWait(webDriver, TimeSpan.FromSeconds(3))
            .Until(ExpectedConditions.ElementToBeClickable(By.XPath("//input[@id='order_billing_zip' and @name='order[billing_zip]']")))
            .SendKeys("<ZIP_CODE>");

            IWebElement country = webDriver.FindElement(By.XPath("//select[@id='order_billing_country' and @name='order[billing_country]']"));
            var countrySelect = new SelectElement(country);
            countrySelect.SelectByValue("<COUNTRY_CODE>");

            IWebElement cardType = webDriver.FindElement(By.Id("credit_card_type"));
            var cardTypeSelect = new SelectElement(cardType);
            cardTypeSelect.SelectByText("<CARD_TYPE>");

            new WebDriverWait(webDriver, TimeSpan.FromSeconds(3))
            .Until(ExpectedConditions.ElementToBeClickable(By.XPath("//input[@id='cnb' and @name='credit_card[cnb]']")))
            .SendKeys("<CARD_NUMBER>");

            IWebElement cardMonth = webDriver.FindElement(By.XPath("//select[@id='credit_card_month' and @name='credit_card[month]']"));
            var cardMonthSelect = new SelectElement(cardMonth);
            cardMonthSelect.SelectByText("<CARD_VALID_MONTH>");

            IWebElement cardYear = webDriver.FindElement(By.XPath("//select[@id='credit_card_year' and @name='credit_card[year]']"));
            var cardYearSelect = new SelectElement(cardYear);
            cardYearSelect.SelectByText("<CARD_VALID_YEAR>");

            new WebDriverWait(webDriver, TimeSpan.FromSeconds(3))
            .Until(ExpectedConditions.ElementToBeClickable(By.XPath("//input[@id='vval' and @name='credit_card[vval]']")))
            .SendKeys("<CARD_CCV>");

            var TermsDiv = webDriver.FindElements(By.XPath("//div[@class='icheckbox_minimal']"));

            foreach(var term in TermsDiv) {
                term.Click();
            }

            //By @by = By.XPath("//div[contains(@class,'icheckbox_minimal')]");
            //new WebDriverWait(webDriver, TimeSpan.FromSeconds(3)).Until(ExpectedConditions.ElementExists(@by));
            //Console.WriteLine($"checkBox: {TermsDiv.GetAttribute("InnerHTML")}");
            


        }

        public void OpenWebBrowser(string url)
        {
            Process myProcess = new Process();

            try
            {
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = url;
                myProcess.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
