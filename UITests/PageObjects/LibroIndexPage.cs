using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;

namespace UITests.PageObjects
{
    public class LibroIndexPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public LibroIndexPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // Page elements
        private IWebElement PageTitle => _driver.FindElement(By.TagName("h1"));
        private IWebElement LibrosTable => _driver.FindElement(By.CssSelector("table.table"));
        private IWebElement NoLibrosAlert => _driver.FindElement(By.CssSelector(".alert"));
        
        // Select All functionality
        private IWebElement SelectAllCheckbox => _driver.FindElement(By.CssSelector("thead input[type='checkbox'], .select-all-checkbox"));
        private IList<IWebElement> IndividualCheckboxes => _driver.FindElements(By.CssSelector("tbody input[type='checkbox']"));
        
        // Get all book rows from the table
        private IList<IWebElement> LibroRows => _driver.FindElements(By.CssSelector("table.table tbody tr"));
        
        // Get edit and delete buttons for a specific book row
        private IWebElement GetEditButton(IWebElement libroRow) => libroRow.FindElement(By.CssSelector("a.btn-outline-primary"));
        private IWebElement GetDeleteButton(IWebElement libroRow) => libroRow.FindElement(By.CssSelector("a.btn-outline-danger"));

        // Navigation methods
        public void NavigateToLibroIndexPage()
        {
            try
            {
                Console.WriteLine($"Navigating to: {TestConfig.BaseUrl}/Libro");
                _driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/Libro");
                
                // Wait for the page to load
                _wait.Until(driver => driver.FindElement(By.TagName("h1")).Displayed);
                Console.WriteLine("Successfully navigated to Libro Index page");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to navigate to Libro Index page: {ex.Message}");
                Console.WriteLine($"Current URL: {_driver.Url}");
                throw;
            }
        }

        public bool IsOnIndexPage()
        {
            try
            {
                return PageTitle.Text.Contains("Libros") && _driver.Url.Contains("/Libro");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool HasLibros()
        {
            try
            {
                return LibroRows.Count > 0;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void SelectAllLibros()
        {
            try
            {
                if (!HasLibros())
                {
                    throw new InvalidOperationException("No hay libros disponibles para seleccionar");
                }

                Console.WriteLine("Selecting all libros from the list");
                
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
                
                Console.WriteLine("Successfully selected all libros");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error selecting all libros: {ex.Message}");
                throw;
            }
        }

        public bool AreAllLibrosSelected()
        {
            try
            {
                if (!HasLibros())
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
                        return selectedRows.Count == LibroRows.Count;
                    }
                    
                    bool allSelected = checkboxes.All(cb => cb.Selected);
                    Console.WriteLine($"Individual checkboxes all selected: {allSelected}");
                    return allSelected;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if all libros are selected: {ex.Message}");
                return false;
            }
        }

        public void ClickDeleteForFirstLibro()
        {
            try
            {
                if (!HasLibros())
                {
                    throw new InvalidOperationException("No hay libros disponibles para eliminar");
                }

                var firstLibroRow = LibroRows.First();
                var deleteButton = GetDeleteButton(firstLibroRow);
                
                Console.WriteLine("Clicking delete for first libro from the list");
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
                return _driver.Url.Contains("/Libro/Delete");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int GetLibroCount()
        {
            try
            {
                return LibroRows.Count;
            }
            catch (NoSuchElementException)
            {
                return 0;
            }
        }

        public string GetFirstLibroName()
        {
            try
            {
                if (!HasLibros())
                {
                    return string.Empty;
                }

                var firstLibroRow = LibroRows.First();
                var nameCell = firstLibroRow.FindElement(By.CssSelector("td:first-child"));
                return nameCell.Text;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting libro name: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
