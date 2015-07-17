﻿using OpenQA.Selenium;
using SpecFlow.Extensions.Web;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SpecFlow.Extensions.WebDriver
{
    abstract public class WrapWebDriver : IWrapWebDriver
    {
        private const int MAX_RETRIES = 3;
        public WrapWebDriver(IWebDriver driver)
        {
            Driver = driver;
        }
        public IWebDriver Driver { get; private set; }

        protected bool _loggedIn;
        public bool LoggedIn
        {
            get
            {
                return _loggedIn;
            }
        }

        abstract public void Login();

        abstract public void Logout();

        abstract  public void NavigateTo(string url);

        public void WaitForPageLoad()
        {
            Driver.WaitForPageLoad();
        }

        public bool Exists(ByEx id)
        {
            return Driver.Exists(id);
        }

        public IWebElement Find(ByEx id)
        {
            return Driver.Find(id);
        }

        public IEnumerable<IWebElement> FindAll(ByEx id)
        {
            return Driver.FindAll(id);
        }

        public void Click(ByEx id)
        {
            TryAgain(() =>
            {
                Find(id).Click();
                return true;
            });
        }
        private void ClickInvisible(ByEx id)
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].Click()", Find(id));
        }
        public void Select(ByEx id)
        {
            TryAgain(() =>
            {
                Find(id).Click();
                return Find(id).Selected;
            });
        }
        public void SendKeys(ByEx id, string text)
        {
            TryAgain(() =>
            {
                Find(id).SendKeys(text);
                return true;
            });
        }
        public void Clear(ByEx id)
        {
            TryAgain(() =>
            {
                Find(id).Clear();
                return true;
            });
        }
        public bool Displayed(ByEx id)
        {
            return TryAgain(() =>
            {
                return Find(id).Displayed;
            });
        }

        public bool TryAgain(Func<bool> func)
        {
            int tryCount = 0;
            bool success = false;
            Exception lastException = null;
            while (tryCount < MAX_RETRIES && !success)
            {
                try
                {
                    success = func();
                }
                catch (Exception e)
                {
                    lastException = e;
                    success = false;
                }
                tryCount++;
                if (!success)
                {
                    Thread.Sleep(1000);
                }
                }

            if (!success && lastException != null)
            {
                throw lastException;
            }

            return success;
        }
    }
}