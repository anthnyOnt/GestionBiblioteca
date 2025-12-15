using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;

namespace UITests.PageObjects
{
    public class EjemplarIndexPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public EjemplarIndexPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // Page elements
        private IWebElement PageTitle => _driver.FindElement(By.TagName("h1"));
        private IWebElement EjemplaresTable => _driver.FindElement(By.CssSelector("table.table"));
        private IWebElement NoEjemplaresAlert => _driver.FindElement(By.CssSelector(".alert"));
        
        // Select All functionality
        private IWebElement SelectAllCheckbox => _driver.FindElement(By.CssSelector("thead input[type='checkbox'], .select-all-checkbox"));
        private IList<IWebElement> IndividualCheckboxes => _driver.FindElements(By.CssSelector("tbody input[type='checkbox']"));
        
        // Get all ejemplar rows from the table
        private IList<IWebElement> EjemplarRows => _driver.FindElements(By.CssSelector("table.table tbody tr"));
        
        // Get edit and delete buttons for a specific ejemplar row
        private IWebElement GetEditButton(IWebElement ejemplarRow) => ejemplarRow.FindElement(By.CssSelector("a.btn-outline-primary"));
        private IWebElement GetDeleteButton(IWebElement ejemplarRow) => ejemplarRow.FindElement(By.CssSelector("a.btn-outline-danger"));

        // Navigation methods
        public void NavigateToEjemplarIndexPage()
        {
            try
            {
                Console.WriteLine($"Navigating to: {TestConfig.BaseUrl}/Ejemplar");
                _driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/Ejemplar");
                
                // Wait for the page to load
                _wait.Until(driver => driver.FindElement(By.TagName("h1")).Displayed);
                Console.WriteLine("Successfully navigated to Ejemplar Index page");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to navigate to Ejemplar Index page: {ex.Message}");
                Console.WriteLine($"Current URL: {_driver.Url}");
                throw;
            }
        }

        public bool IsOnIndexPage()
        {
            try
            {
                return PageTitle.Text.Contains("Ejemplares") && _driver.Url.Contains("/Ejemplar");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool HasEjemplares()
        {
            try
            {
                return EjemplarRows.Count > 0;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void SelectAllEjemplares()
        {
            try
            {
                if (!HasEjemplares())
                {
                    throw new InvalidOperationException("No hay ejemplares disponibles para seleccionar");
                }

                Console.WriteLine("Selecting all ejemplares from the list");
                
                // Try to find and click a "Select All" checkbox
                try
                {
                    SelectAllCheckbox.Click();
                    Console.WriteLine("Clicked Select All checkbox");
                }
                catch (NoSuchElementException)
                {
                    // If no Select All checkbox exists, manually select all individual checkboxes
                    Console.WriteLine("No Select All checkbox found, selecting individual checkboxes");
                    foreach (var checkbox in IndividualCheckboxes)
                    {
                        if (!checkbox.Selected)
                        {
                            checkbox.Click();
                        }
                    }
                }
                
                Console.WriteLine("Successfully selected all ejemplares");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error selecting all ejemplares: {ex.Message}");
                throw;
            }
        }

        public bool AreAllEjemplaresSelected()
        {
            try
            {
                if (!HasEjemplares())
                {
                    return false;
                }

                // Check if Select All checkbox is selected
                try
                {
                    if (SelectAllCheckbox.Selected)
                    {
                        Console.WriteLine("Select All checkbox is checked");
                        return true;
                    }
                }
                catch (NoSuchElementException)
                {
                    // Check if all individual checkboxes are selected
                    var checkboxes = IndividualCheckboxes;
                    if (checkboxes.Count == 0)
                    {
                        // If no checkboxes exist, assume selection means all rows are highlighted/active
                        Console.WriteLine("No checkboxes found, checking for visual selection indicators");
                        var selectedRows = _driver.FindElements(By.CssSelector("table.table tbody tr.selected, table.table tbody tr.active"));
                        return selectedRows.Count == EjemplarRows.Count;
                    }
                    
                    bool allSelected = checkboxes.All(cb => cb.Selected);
                    Console.WriteLine($"Individual checkboxes all selected: {allSelected}");
                    return allSelected;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if all ejemplares are selected: {ex.Message}");
                return false;
            }
        }

        public void ClickDeleteForFirstEjemplar()
        {
            try
            {
                if (!HasEjemplares())
                {
                    throw new InvalidOperationException("No hay ejemplares disponibles para eliminar");
                }

                var firstEjemplarRow = EjemplarRows.First();
                var deleteButton = GetDeleteButton(firstEjemplarRow);
                
                Console.WriteLine("Clicking delete for first ejemplar from the list");
                deleteButton.Click();
                
                // Wait for navigation to delete page
                _wait.Until(driver => driver.Url.Contains("/Delete"));
                Console.WriteLine("Successfully navigated to Delete page");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clicking delete: {ex.Message}");
                throw;
            }
        }

        public bool IsOnDeletePage()
        {
            try
            {
                return _driver.Url.Contains("/Ejemplar/Delete");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int GetEjemplarCount()
        {
            try
            {
                return EjemplarRows.Count;
            }
            catch (NoSuchElementException)
            {
                return 0;
            }
        }

        public string GetFirstEjemplarName()
        {
            try
            {
                if (!HasEjemplares())
                {
                    return string.Empty;
                }

                var firstEjemplarRow = EjemplarRows.First();
                var nameCell = firstEjemplarRow.FindElement(By.CssSelector("td:first-child"));
                return nameCell.Text;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting ejemplar name: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
