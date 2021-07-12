using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ksiegarnia
{
    public partial class MainWindow : Window
    {       

        private List<Book> BooksList;
        private bool zaladowane = false; // prawda, gdy książki zostaną dodane do listy

        public MainWindow()
        {
            InitializeComponent();
            BooksList = new List<Book>();
            BooksList.Add(new Book("Warcraft: Dzień smoka", "Richard A. Knaak", 84.61M));
            BooksList.Add(new Book("Warcraft: Władca klanów", "Christie Golden", 120.00M));
            BooksList.Add(new Book("Warcraft: Ostatni strażnik", "Jeff Grubb", 32.80M));
            BooksList.Add(new Book("Warcraft: Wojna Starożytnych", "Richard A. Knaak", 173.80M));
            BooksList.Add(new Book("Warcraft: Of Blood and Honor", "Chris Metzen", 28.99M));
            BooksList.Add(new Book("World of Warcraft: Krąg nienawiści", "Keith R.A. DeCandido", 175.00M));
            BooksList.Add(new Book("World of Warcraft: Narodziny Hordy", "Christie Golden", 26.96M));
            BooksList.Add(new Book("Thrall: Twilight of the Aspects", "Christie Golden", 32.72M));
            BooksList.Add(new Book("Jaina Proudmoore: Wichry wojny", "Christie Golden", 37.67M));
            BooksList.Add(new Book("Illidan", "William King", 35.99M));
            BooksList.Add(new Book("World of Warcraft: Zbrodnie wojenne", "Christie Golden", 28.97M));
            BooksList.Add(new Book("World of Warcraft: Arthas. Przebudzenie Króla Lisza", "Christie Golden", 27.97M));

            DisplayBooks.ItemsSource = BooksList;
            zaladowane = true;
        }

        //   ---   SORTOWANIE TYTUŁÓW PO CENIE   ---   //
        private void maxCena(object sender, TextChangedEventArgs e)
        {

            decimal maxCost;
            
            if (Decimal.TryParse(maxCostFilter.Text, out maxCost))
            {
                ListCollectionView view = CollectionViewSource.GetDefaultView(DisplayBooks.ItemsSource) as ListCollectionView;
                ProductByPriceFilter filter = new ProductByPriceFilter(maxCost);
                view.Filter = filter.FilterItem;
            }
        }

        //   ---   SORTOWANIE TYTUŁÓW  PO NAZWIE   ---   //
        private void zmienSortowanie(object sender, SelectionChangedEventArgs e)
        {
            if (zaladowane)
            {
                ListCollectionView view;
                view = CollectionViewSource.GetDefaultView(DisplayBooks.ItemsSource) as ListCollectionView;
                switch (SortMethodsList.SelectedIndex)
                {
                    case 0:
                        view.SortDescriptions.Clear();
                        break;
                    case 1:
                        view.SortDescriptions.Clear();
                        view.SortDescriptions.Add(new SortDescription("BookTitle", ListSortDirection.Ascending));
                        break;
                    case 2:
                        view.SortDescriptions.Clear();
                        view.SortDescriptions.Add(new SortDescription("BookTitle", ListSortDirection.Descending));
                        break;
                    default:
                        break;
                }
            }
        }

        //   ---   BŁĄD WALIDACJI   ---   //

        private void bladWalidacji(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                MessageBox.Show(e.Error.ErrorContent.ToString());
            }
        }      
    }

    //   ------------------   //
    //   ---   WYKŁAD   ---   //
    //   ------------------   //

    //   ---   KLASA BOOK   ---   //
    public class Book
    {
        public Book() { }
        public Book(string BookTitle, string BookAuthor, decimal BookPrice)
        {
            this.BookTitle = BookTitle;
            this.BookAuthor = BookAuthor;
            this.BookPrice = BookPrice;
        }

        public string BookTitle { get; set; }
        public string BookAuthor { get; set; }
        public decimal BookPrice { get; set; }

    }


    //   ---   SORTOWANIE PO CENIE   ---   //

    public class ProductByPriceFilter
    {
        public decimal MaxPrice { get; set; }
        public ProductByPriceFilter(decimal maxPrice)
        {
            MaxPrice = maxPrice;
        }
        public bool FilterItem(Object item)
        {
            Book product = item as Book;
            if (product != null)
            {
                return (product.BookPrice < MaxPrice);
            }
            return false;
        }
    }


    //   ---   FORMATOWANIE CENY   ---   //
    [ValueConversion(typeof(decimal), typeof(string))]
    public class PriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal price = (decimal)value;
            return price.ToString("C", culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string price = value.ToString();
            decimal result;
            if (Decimal.TryParse(price, NumberStyles.Any,
            culture, out result))
            {
                return result;
            }
            return value;
        }
    }


    //   ---   FORMATOWANIE TŁA   ---   //
    [ValueConversion(typeof(decimal), typeof(string))]
    public class PriceToBackgroundConverter : IValueConverter
    {
        public decimal MaximumPriceToHighlight { get; set; }
        public Brush HighlightBrush { get; set; }
        public Brush DefaultBrush { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((decimal)value <= MaximumPriceToHighlight) ? HighlightBrush : DefaultBrush;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string price = value.ToString();
            decimal result;
            if (Decimal.TryParse(price, NumberStyles.Any, culture, out result))
            {
                return result;
            }
            return value;
        }
    }

    //   ---   ZASADY WALIDACJI   ---   //
    public class PositivePriceRule : ValidationRule
    {
        private decimal min = 0;
        private decimal max = Decimal.MaxValue;
        public decimal Min
        {
            get { return min; }
            set { min = value; }
        }
        public decimal Max
        {
            get { return max; }
            set { max = value; }
        }

        //   ---   WALIDACJA   ---   //
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            decimal price = 0;
            try
            {
                if (((string)value).Length > 0)
                    price = Decimal.Parse((string)value, NumberStyles.Any, cultureInfo);
            }
            catch
            {
                return new ValidationResult(false, "illegal Characters");
            }
            if ((price < Min) || (price > Max))
            {
                return new ValidationResult(false,  "Not in the range " + Min + " to " + Max + ".");
            }
            else
            {
                return new ValidationResult(true, null);
            }

        }
    }

}
