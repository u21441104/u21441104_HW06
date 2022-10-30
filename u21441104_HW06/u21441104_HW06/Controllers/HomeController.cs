using u21441104_HW06.ViewModels;
using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Dynamic;

namespace u21441104_HW06.Controllers
{
    public class HomeController : Controller
    {
        private SqlConnection connection = new SqlConnection(@"Data Source=TYRONSSPEEDYBOY\SQLEXPRESS02;Initial Catalog=BikeStores;Integrated Security=True");

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Orders()
        {
            dynamic myModel = new ExpandoObject(); //Allows multiple models to be sent

                myModel.info = getInfo();
                myModel.order = getOrd();
            

            return View(myModel);
        }

        [HttpPost]
        public ActionResult Orders(DateTime searchString)
        {
            dynamic myModel = new ExpandoObject(); //Allows multiple models to be sent

            if (searchString == null)
            {

                myModel.info = getInfo();
                myModel.order = getOrd();

            }
            else
            {
                List<OrderVM> Order = getOrd().Where(x => x.OrderDate.Equals(searchString)).ToList();
                List<OrderDetailsVM> orderDetails = getInfo().Where(x => x.OrderDate.Equals(searchString)).ToList();
                myModel.info = orderDetails;
                myModel.order = Order;
            }

            return View(myModel);
        }

        public List<OrderDetailsVM> getInfo()
        {
            List<OrderDetailsVM> OrderInfoList = new List<OrderDetailsVM>(); //contains OrdderID and date to allow for fomatting in the view

            try
            {
                connection.Open();
                SqlCommand myCommand = new SqlCommand("select order_id, order_date from sales.orders", connection);
                SqlDataReader reader = myCommand.ExecuteReader();

                while (reader.Read())
                {
                    OrderDetailsVM OrderInfo = new OrderDetailsVM();

                    OrderInfo.OrderID = reader.GetInt32(0);
                    OrderInfo.OrderDate = reader.GetDateTime(1);

                    OrderInfoList.Add(OrderInfo);
                }
            }
            catch (Exception error)
            {
                ViewBag.Status = 0;
            }
            finally
            {
                connection.Close();
            }

            return OrderInfoList;
        }

        public List<OrderVM> getOrd()
        {
            List<OrderVM> orderList = new List<OrderVM>(); //send list of orders to view

            try
            {
                connection.Open();
                SqlCommand myCommand = new SqlCommand("Select product_name, i.list_price, quantity, i.order_id, order_date from sales.order_items as i inner join production.products as p on i.product_id = p.product_id inner join sales.orders as o on i.order_id = o.order_id", connection);
                SqlDataReader reader = myCommand.ExecuteReader();

                while (reader.Read())
                {
                    OrderVM order = new OrderVM();

                    order.ProductName = reader.GetString(0);
                    order.ListPrice = reader.GetDecimal(1);
                    order.Quantity = reader.GetInt32(2);
                    order.OrderID = reader.GetInt32(3);
                    order.OrderDate = reader.GetDateTime(4);

                    orderList.Add(order);
                }
            }
            catch (Exception error)
            {
                ViewBag.Status = 0;
            }
            finally
            {
                connection.Close();
            }

            return orderList;
        }

        public ActionResult Products()
        {
            List<ProductsVM> prodList = new List<ProductsVM>(); //Display products on view

            try
            {
                connection.Open();
                SqlCommand myCommand = new SqlCommand("select product_name, model_year, list_price, brand_name, category_name from production.products, production.categories, production.brands where production.products.category_id = production.categories.category_id and production.products.brand_id = production.brands.brand_id", connection);
                SqlDataReader reader = myCommand.ExecuteReader();

                while (reader.Read())
                {
                    ProductsVM product = new ProductsVM();

                    product.Name = reader.GetString(0);
                    product.Year = reader.GetInt32(1);
                    product.Price = reader.GetDecimal(2);
                    product.Brand = reader.GetString(3);
                    product.Category = reader.GetString(4);

                    prodList.Add(product);
                }
            }
            catch (Exception errpr)
            {
                ViewBag.Status = 0;
            }
            finally
            {
                connection.Close();
            }

            return View(prodList);
        }

        public ActionResult Report()
        {
            return View();
        }
    }
}