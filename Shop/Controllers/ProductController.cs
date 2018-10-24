using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Shop.Models;
using System.Text.RegularExpressions;
using System.Data.Entity;
using System.IO;

namespace Shop.Controllers
{
    public class ProductController : Controller
    {
        private ShopEntities shop = new ShopEntities();
        public int count = 0;
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetProducts()
        {
            ShopEntities shop = new ShopEntities();
            if (count == 0)
            {
                count++;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://www.tashirpizza.am/eng/menu?___from_store=arm");
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                HtmlDocument doc = new HtmlDocument();
                doc.Load(resp.GetResponseStream());
                HtmlNode wrapper = doc.DocumentNode.SelectSingleNode("//div[@class='controls-wrapper']");
                HtmlNode menu = wrapper.SelectSingleNode("//div[@class='container menu-container controls-menu']");
                HtmlNode ul = menu.SelectSingleNode("//ul[@class='controls']");
                string datafilter;
                foreach (HtmlNode li in ul.SelectNodes("li"))
                {
                    datafilter = li.GetAttributeValue("data-filter", null);
                    li.SelectSingleNode("//span");
                    string newdatafilter = datafilter;
                    string productname = li.InnerText.Trim();
                    shop.Produts.Add(new Produts() { ProductName = productname });
                    shop.SaveChanges();
                    int id = 0;
                    if (datafilter.Contains('.') == true)
                    {
                        foreach (var product in shop.Produts)
                        {
                            if (product.ProductName == productname)
                            {
                                id = product.Product_Id;
                            }
                        }
                        shop.ProductType.Add(new ProductType() { ProductTypeName = productname, Product_Id = id });
                        shop.SaveChanges();
                        newdatafilter = datafilter.Substring(1);
                        HtmlNode container = doc.DocumentNode.SelectSingleNode("//div[@class='container']");
                        string path = null;
                        string innerabout = null;
                        string name = null;
                        string type = null;
                        int? price = null;
                        int producttypeid = 0;
                        int concreteid = 0;
                        foreach (HtmlNode concrete in container.SelectNodes($"div[@class='mix {newdatafilter}']"))
                        {
                            List<string> types = new List<string>();
                            List<int?> prices = new List<int?>();
                            HtmlNode block = concrete.SelectSingleNode("div[@class='item-block']");
                            HtmlNode desk = block.SelectSingleNode("div[@class='desc-block']");
                            HtmlNode imageblock = block.SelectSingleNode("div[@class='img-block']");
                            foreach (var imag in imageblock.ChildNodes)
                            {
                                if (imag.HasClass("quickview"))
                                {
                                    path = imag.SelectSingleNode("img").GetAttributeValue("data-src", null);
                                }
                            }
                            foreach (var product in desk.ChildNodes)
                            {
                                if (product.HasClass("list-product-desc"))
                                {
                                    innerabout = product.InnerText.Trim();
                                }
                                if (product.HasClass("list-product-name"))
                                {
                                    name = product.InnerText.Trim();
                                }
                                if (product.HasClass("product-options"))
                                {
                                    HtmlNode select = product.SelectSingleNode("select");

                                    foreach (var option in select.SelectNodes("option"))
                                    {
                                        int va;
                                        bool c = int.TryParse(Regex.Match(option.InnerText, @"\d+").Value.Trim(), out va);
                                        if (c)
                                        {
                                            price = va;
                                        }
                                        int sub = option.InnerText.IndexOf('(');
                                        type = option.InnerText.Substring(0, sub).Trim();
                                        if (price != null)
                                        {
                                            prices.Add(price);
                                        }
                                        if (type != null)
                                        {
                                            types.Add(type);
                                        }
                                    }
                                }
                                else if (product.Name == "form")
                                {
                                    HtmlNode nogrouped = product.SelectSingleNode("div[@class='no_grouped_name fl']");
                                    HtmlNode childofnogrouped = nogrouped.SelectSingleNode("div[@class='no_grouped_name_label']");
                                    HtmlNodeCollection spans = childofnogrouped.ChildNodes;
                                    int va;
                                    bool c = int.TryParse(Regex.Match(spans[3].InnerText, @"\d+").Value.Trim(), out va);
                                    type = spans[1].InnerText.Trim();
                                    price = va;
                                    prices.Add(price);
                                    types.Add(type);
                                }
                            }
                            foreach (var productType in shop.ProductType)
                            {
                                if (productType.ProductTypeName == productname)
                                {
                                    producttypeid = productType.ProductType_Id;
                                }
                            }
                            shop.ConcreteProduct.Add(new ConcreteProduct() { ConcreteProductName = name, About = innerabout, ImagePath = path, ProductType_Id = producttypeid });
                            shop.SaveChanges();
                            foreach (var concret in shop.ConcreteProduct)
                            {
                                if (concret.ConcreteProductName == name)
                                {
                                    concreteid = concret.ConcreteProduct_Id;
                                }
                            }
                            for (int i = 0; i < types.Count; i++)
                            {
                                shop.ConcreteProductDetails.Add(new ConcreteProductDetails() { ConcreteProductTypeName = types[i], Price = prices[i], ConcreteProduct_Id = concreteid });
                                shop.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        HtmlNode glob = doc.DocumentNode.SelectSingleNode("//div[@class='container menu-container controls-menu']");
                        HtmlNode secondlevel = glob.SelectSingleNode("//div[@class='secon-level']");
                        foreach (HtmlNode div in secondlevel.SelectNodes("div"))
                        {
                            if (div.GetClasses().Contains(datafilter))
                            {
                                foreach (HtmlNode but in div.ChildNodes)
                                {

                                    HtmlNode container = doc.DocumentNode.SelectSingleNode("//div[@class='container']");
                                    string value = but.GetAttributeValue("data-filter", null);
                                    string productypename = but.InnerText.Trim();
                                    if (value == null)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        foreach (var product in shop.Produts)
                                        {
                                            if (product.ProductName == productname)
                                            {
                                                id = product.Product_Id;
                                            }
                                        }
                                        shop.ProductType.Add(new ProductType() { ProductTypeName = productypename, Product_Id = id });
                                        shop.SaveChanges();
                                        string val = value.Substring(1);
                                        string path = null;
                                        string innerabout = null;
                                        string name = null;
                                        string type = null;
                                        int? price = null;
                                        int producttypeid = 0;
                                        int concreteid = 0;
                                        foreach (HtmlNode concrete in container.SelectNodes($"div[@class='mix {val}']"))
                                        {
                                            HtmlNode block = concrete.SelectSingleNode("div[@class='item-block']");
                                            HtmlNode desk = block.SelectSingleNode("div[@class='desc-block']");
                                            HtmlNode imageblock = block.SelectSingleNode("div[@class='img-block']");
                                            List<string> types = new List<string>();
                                            List<int?> prices = new List<int?>();
                                            foreach (var imag in imageblock.ChildNodes)
                                            {
                                                if (imag.HasClass("quickview"))
                                                {
                                                    path = imag.SelectSingleNode("img").GetAttributeValue("data-src", null);
                                                }
                                            }
                                            foreach (var product in desk.ChildNodes)
                                            {
                                                if (product.HasClass("list-product-desc"))
                                                {
                                                    innerabout = product.InnerText.Trim();
                                                }
                                                if (product.HasClass("list-product-name"))
                                                {
                                                    name = product.InnerText.Trim();
                                                }
                                                if (product.HasClass("product-options"))
                                                {
                                                    HtmlNode select = product.SelectSingleNode("select");
                                                    foreach (var option in select.SelectNodes("option"))
                                                    {
                                                        int va;
                                                        bool c = int.TryParse(Regex.Match(option.InnerText, @"\d+").Value.Trim(), out va);
                                                        if (c)
                                                        {
                                                            price = va;
                                                        }
                                                        int sub = option.InnerText.IndexOf('(');
                                                        type = option.InnerText.Substring(0, sub).Trim();
                                                        if (price != null)
                                                        {
                                                            prices.Add(price);
                                                        }
                                                        if (type != null)
                                                        {
                                                            types.Add(type);
                                                        }
                                                    }
                                                }
                                                else if (product.Name == "form")
                                                {
                                                    HtmlNode nogrouped = product.SelectSingleNode("div[@class='no_grouped_name fl']");
                                                    HtmlNode childofnogrouped = nogrouped.SelectSingleNode("div[@class='no_grouped_name_label']");
                                                    HtmlNodeCollection spans = childofnogrouped.ChildNodes;
                                                    int va;
                                                    bool c = int.TryParse(Regex.Match(spans[3].InnerText, @"\d+").Value.Trim(), out va);
                                                    type = spans[1].InnerText.Trim();
                                                    price = va;
                                                    prices.Add(price);
                                                    types.Add(type);
                                                }
                                            }
                                            foreach (var productType in shop.ProductType)
                                            {
                                                if (productType.ProductTypeName == productypename)
                                                {
                                                    producttypeid = productType.ProductType_Id;
                                                }
                                            }
                                            shop.ConcreteProduct.Add(new ConcreteProduct() { ConcreteProductName = name, About = innerabout, ImagePath = path, ProductType_Id = producttypeid });
                                            shop.SaveChanges();
                                            foreach (var concret in shop.ConcreteProduct)
                                            {
                                                if (concret.ConcreteProductName == name)
                                                {
                                                    concreteid = concret.ConcreteProduct_Id;
                                                }
                                            }
                                            for (int i = 0; i < types.Count; i++)
                                            {
                                                shop.ConcreteProductDetails.Add(new ConcreteProductDetails() { ConcreteProductTypeName = types[i], Price = prices[i], ConcreteProduct_Id = concreteid });
                                                shop.SaveChanges();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return View("Products", shop.Produts);
            }
            return View("Products", shop.Produts);
        }
        public ActionResult CreateProduct()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateProduct(Produts product)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in shop.Produts)
                {
                    if (item.ProductName==product.ProductName)
                    {
                        return View("WithSameParams");
                    }
                }
                shop.Produts.Add(product);
                shop.ProductType.Add(new ProductType() { ProductTypeName = product.ProductName, Product_Id = product.Product_Id });
                shop.SaveChanges();
                return View("Products", shop.Produts);
            }
                return View("Invalid");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            Produts produt=shop.Produts.Find(id);
            return View(produt);
        }
        [HttpPost]
        public ActionResult Edit(Produts product)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in shop.Produts)
                {
                    if (item.ProductName==product.ProductName)
                    {
                        return View("WithSameParams");
                    }
                }
                foreach (var item in shop.Produts)
                {
                    if (item.Product_Id == product.Product_Id)
                    {
                        item.ProductName = product.ProductName;
                    }
                }
                shop.SaveChanges();
                return View("Products", shop.Produts);
            }
            return View("Invalid");
        }
        [HttpGet]
        public ActionResult HomePage()
        {
            return View("Products",shop.Produts);
        }
        public ActionResult GetTypes(int id)
        {
            int proid = shop.Produts.Find(id).Product_Id;
            string name = shop.Produts.Find(id).ProductName;
            List<ProductType> pro=new List<ProductType>();
            foreach (var item in shop.ProductType)
            {
                if (item.Product_Id==id)
                {
                    pro.Add(item);
                }
            }
            if (pro.Count == 0)
            {
                Produts product = new Produts();
                product.Product_Id = proid;
                product.ProductName = name;
                return View("NullProTypes", product);
            }
            return View(pro);
        }
        [HttpGet]
        public ActionResult GetConcreateProduct(int typeid)
        {

            if (GetByTypeId(typeid).Count==0)
            {
                ProductWithAllDetails productWithAll = new ProductWithAllDetails() { ProductTypeId = typeid, Name = "default", Prices = null, Types = null, Path = "default", About = "default" };
                return View("NotFoundCreateNew",productWithAll);
            }
            return View(GetByTypeId(typeid));
        }
        [HttpGet]
        public ActionResult EditTypeOfProduct(int id)
        {
            ProductType produttype = shop.ProductType.Find(id);
            return View(produttype);
        }
        [HttpPost]
        public ActionResult EditTypeOfProduct(ProductType productType)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in shop.ProductType)
                {
                    if (item.ProductTypeName==productType.ProductTypeName && item.Product_Id==productType.Product_Id)
                    {
                        return View("WithSameParams");
                    }
                }
                int id = 0;
                foreach (var item in shop.ProductType)
                {
                    if (productType.ProductType_Id == item.ProductType_Id)
                    {
                        item.ProductTypeName = productType.ProductTypeName;
                        id = item.Product_Id;
                    }
                }
                shop.SaveChanges();
                List<ProductType> pro = new List<ProductType>();
                foreach (var item in shop.ProductType)
                {
                    if (item.Product_Id == id)
                    {
                        pro.Add(item);
                    }
                }
                return View("GetTypes", pro);
            }
            return View("Invalid");
        }
        [HttpGet]
        public ActionResult DeleteProduct(int id)
        {
            Produts produt = shop.Produts.Find(id);
            return View(produt);
        }
        [HttpPost]
        public ActionResult DeletProduct(int id)
        {
            foreach (var item in shop.ProductType)
            {
                if (item.Product_Id==id)
                {
                foreach (var con in shop.ConcreteProduct)
                {
                    if (con.ProductType_Id==item.ProductType_Id)
                    {
                        foreach (var ite in shop.ConcreteProductDetails)
                        {
                            if (ite.ConcreteProduct_Id==con.ConcreteProduct_Id)
                            {
                                shop.ConcreteProductDetails.Remove(ite);
                            }
                        }
                        shop.ConcreteProduct.Remove(con);
                    }
                }
                shop.ProductType.Remove(item);
                }
            }
            foreach (var item1 in shop.Produts)
            {
                if (item1.Product_Id==id)
                {
                    shop.Produts.Remove(item1);
                }
            }
            shop.SaveChanges();
            return View("Products",shop.Produts);
        }
        [HttpGet]
        public ActionResult Cancel()
        {
            return View("Products",shop.Produts);
        }
        public ActionResult DeleteTypeofProduct(int id)
        {
            ProductType productype = shop.ProductType.Find(id);
            return View(productype);
        }
        [HttpPost]
        public ActionResult Cancel(int id)
        {
            ProductType protype=shop.ProductType.Find(id);
            int Id_Concrete = protype.Product_Id;
            List<ProductType> pro = new List<ProductType>();
            foreach (var item in shop.ProductType)
            {
                if (item.Product_Id == Id_Concrete)
                {
                    pro.Add(item);
                }
            }
            return View("GetTypes",pro);
        }
        public ActionResult DeletTypeofProduct(int id)
        {
            foreach (var conc in shop.ConcreteProduct)
            {
                if (conc.ProductType_Id==id)
                {
                    foreach (var item in shop.ConcreteProductDetails)
                    {
                        if (conc.ConcreteProduct_Id==item.ConcreteProduct_Id)
                        {
                            shop.ConcreteProductDetails.Remove(item);
                        }
                    }
                    shop.ConcreteProduct.Remove(conc);
                }
            }
            int idpro = 0;
            foreach (var item in shop.ProductType)
            {
                if (item.ProductType_Id==id)
                {
                    idpro = item.Product_Id;
                    shop.ProductType.Remove(item);
                }
            }
            shop.SaveChanges();
            List<ProductType> pro = new List<ProductType>();
            foreach (var item in shop.ProductType)
            {
                if (item.Product_Id == idpro)
                {
                    pro.Add(item);
                }
            }
            if (pro.Count==0)
            {
                foreach (var item in shop.Produts)
                {
                    if (item.Product_Id==idpro)
                    {
                        return View("NullProTypes",item);
                    }
                }
            }
            return View("GetTypes",pro);
        }
        public ActionResult AddType(int id)
        {
            ProductType product = new ProductType();
            product.Product_Id = id;
            return View(product);
        }
        [HttpPost]
        public ActionResult AddType(int id,ProductType product)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in shop.ProductType)
                {
                    if (item.Product_Id==id && item.ProductTypeName==product.ProductTypeName)
                    {
                        return View("WithSameParams");
                    }
                }
                product.Product_Id = id;
                shop.ProductType.Add(product);
                shop.SaveChanges();
                List<ProductType> pro = new List<ProductType>();
                foreach (var item in shop.ProductType)
                {
                    if (item.Product_Id == product.Product_Id)
                    {
                        pro.Add(item);
                    }
                }
                return View("GetTypes", pro);
            }
            return View("Invalid");
        }       
        public ActionResult RequestForDeletingConcreteProduct(ProductWithAllDetails allDetails)
        {
            return View("DeleteConcreteProduct",allDetails);
        }
        public ActionResult DeleteConcreateProduct(int id)
        {
            int typeid=0;
            foreach (var item in shop.ConcreteProductDetails)
            {
                if (item.ConcreteProduct_Id==id)
                {
                    shop.ConcreteProductDetails.Remove(item);
                }
            }
            shop.SaveChanges();
            foreach (var item in shop.ConcreteProduct)
            {
                if (item.ConcreteProduct_Id==id)
                {
                    typeid = item.ProductType_Id;
                    shop.ConcreteProduct.Remove(item);
                }
            }
            shop.SaveChanges();
            if (GetByTypeId(typeid).Count==0)
            {
                ProductWithAllDetails productWithAll = new ProductWithAllDetails() { ProductTypeId = typeid };
                return View("NotFoundCreateNew", productWithAll);
            }
            return View("GetConcreateProduct",GetByTypeId(typeid));
        }
        public ActionResult CancelDeletingConcreteProduct(int id)
        {
            
            return View("GetConcreateProduct", GetByTypeId(id));
        }
        [HttpPost]
        public ActionResult CreateNew(int typeid, HttpPostedFileBase image, ProductWithAllDetails productWithAll)
        {
                foreach (var item in shop.ConcreteProduct)
                {
                    if (productWithAll.ProductTypeId==item.ProductType_Id && item.ConcreteProductName==item.ConcreteProductName)
                    {
                        return View("WithSameParams");
                    }
                }
                ConcreteProduct concrete = new ConcreteProduct();
                if (image != null && image.ContentLength > 0)
                {
                    string file_name = productWithAll.Name + Path.GetExtension(image.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/image"), file_name);
                    image.SaveAs(path);
                    concrete.ImagePath = $"http://localhost:52850/Content/image/{file_name}";
                }
                else
                {
                    concrete.ImagePath = "http://www.tashirpizza.am/media/catalog/product/cache/1/small_image/318x250/040ec09b1e35df139433887a97daa66f/images/catalog/product/placeholder/small_image.jpg";
                }
                concrete.ConcreteProductName = productWithAll.Name;
                concrete.About = productWithAll.About;
                concrete.ProductType_Id = typeid;
                shop.ConcreteProduct.Add(concrete);
                shop.SaveChanges();
                int id = 0;
                foreach (var item in shop.ConcreteProduct)
                {
                    if (item.ConcreteProductName == concrete.ConcreteProductName)
                    {
                        id = item.ConcreteProduct_Id;
                    }
                }
                for (int i = 0; i < productWithAll.Prices.Count; i++)
                {
                    ConcreteProductDetails details = new ConcreteProductDetails();
                    details.Price = productWithAll.Prices[i];
                    details.ConcreteProductTypeName = productWithAll.Types[i];
                    details.ConcreteProduct_Id = id;
                    shop.ConcreteProductDetails.Add(details);
                }
                shop.SaveChanges();
                return View("GetConcreateProduct", GetByTypeId(typeid));
        }
        public ActionResult CreateNew(int id)
        {
            ProductWithAllDetails productWithAll = new ProductWithAllDetails() { ProductTypeId = id, Name = "default", Prices = null, Types = null, Path = "default", About = "default" };
            return View(productWithAll);
        }
        public ActionResult EditConProduct(int id)
        {
            ProductWithAllDetails productWithAll = new ProductWithAllDetails();
            foreach (var item in shop.ConcreteProduct)
            {
                if (item.ConcreteProduct_Id==id)
                {
                    productWithAll.Name = item.ConcreteProductName;
                    productWithAll.Path = item.ImagePath;
                    productWithAll.About = item.About;
                    productWithAll.Id = id;
                    break;
                }
            }
            List<int?> prices = new List<int?>();
            List<string> types = new List<string>();
            foreach (var item in shop.ConcreteProductDetails)
            {
                if (item.ConcreteProduct_Id==id)
                {
                    prices.Add(item.Price);
                    types.Add(item.ConcreteProductTypeName);
                }
            }
            productWithAll.Prices = prices;
            productWithAll.Types = types;
            return View(productWithAll);
        }
        [HttpPost]
        public ActionResult EditConProduct(int id, HttpPostedFileBase image, ProductWithAllDetails productWithAll)
        {
                int typeid = 0;
                foreach (var item in shop.ConcreteProduct)
                {
                
                    if (item.ConcreteProduct_Id == id)
                    {
                        typeid = item.ProductType_Id;
                        item.ConcreteProductName = productWithAll.Name;
                        item.About = productWithAll.About;
                        if (image != null && image.ContentLength > 0)
                        {
                            string file_name = productWithAll.Name + Path.GetExtension(image.FileName);
                            string path = Path.Combine(Server.MapPath("~/Content/image"), file_name);
                            image.SaveAs(path);
                            item.ImagePath = $"http://localhost:52850/Content/image/{file_name}";
                            break;
                        }
                        else if (productWithAll.Path == null)
                        {
                            item.ImagePath = "http://www.tashirpizza.am/media/catalog/product/cache/1/small_image/318x250/040ec09b1e35df139433887a97daa66f/images/catalog/product/placeholder/small_image.jpg";
                            break;
                        }
                        else if (productWithAll.Path != null && productWithAll.Path.EndsWith(".png") || productWithAll.Path.EndsWith(".jpeg") || productWithAll.Path.EndsWith(".gif"))
                        {
                            break;
                        }
                    }
                }
                shop.SaveChanges();
                foreach (var item in shop.ConcreteProductDetails)
                {
                    if (item.ConcreteProduct_Id == id)
                    {
                        shop.ConcreteProductDetails.Remove(item);
                    }
                }
                shop.SaveChanges();
                for (int i = 0; i < productWithAll.Prices.Count; i++)
                {
                    if (productWithAll.Prices[i] != null && productWithAll.Types[i] != null)
                    {
                        shop.ConcreteProductDetails.Add(new ConcreteProductDetails() { ConcreteProductTypeName = productWithAll.Types[i], Price = productWithAll.Prices[i], ConcreteProduct_Id = id });
                    }
                }
                shop.SaveChanges();
                return View("GetConcreateProduct", GetByTypeId(typeid));
        }
        private List<ProductWithAllDetails> GetByTypeId(int typeid)
        {
            List<ProductWithAllDetails> allDetails = new List<ProductWithAllDetails>();
            var idfortype = from a in shop.ConcreteProduct
                            where a.ProductType_Id == typeid
                            join b in shop.ConcreteProductDetails on a.ConcreteProduct_Id equals b.ConcreteProduct_Id
                            select new ProductWithAllDetails() { Id = a.ConcreteProduct_Id,ProductTypeId=a.ProductType_Id ,Name = a.ConcreteProductName, About = a.About, Path = a.ImagePath };
            if (idfortype.Count() == 0)
            {
                var pro = from a in shop.ConcreteProduct
                          where a.ProductType_Id == typeid
                          select new ProductWithAllDetails() { Id = a.ConcreteProduct_Id, ProductTypeId = a.ProductType_Id, Name = a.ConcreteProductName, About = a.About, Path = a.ImagePath };
                allDetails.AddRange(pro.ToList());
                return allDetails;
            }
            else
            {
                foreach (var id in idfortype)
                {
                    List<int?> price = new List<int?>();
                    List<string> types = new List<string>();
                    bool check = true;
                    foreach (var item in shop.ConcreteProductDetails)
                    {
                        if (id.Id == item.ConcreteProduct_Id)
                        {
                            price.Add(item.Price);
                            types.Add(item.ConcreteProductTypeName);
                        }
                    }
                    foreach (var item in allDetails)
                    {
                        if (item.Name == id.Name)
                        {
                            check = false;
                        }
                    }
                    if (check)
                    {
                        allDetails.Add(new ProductWithAllDetails() { Id = id.Id, ProductTypeId = id.ProductTypeId,Name = id.Name, About = id.About, Path = id.Path, Prices = price, Types = types });
                    }
                }
                var pro = from a in shop.ConcreteProduct
                          where a.ProductType_Id == typeid
                          select new ProductWithAllDetails() { Id = a.ConcreteProduct_Id, ProductTypeId = a.ProductType_Id, Name = a.ConcreteProductName, About = a.About, Path = a.ImagePath };
                List<ProductWithAllDetails> union = new List<ProductWithAllDetails>();
                foreach (var item in pro)
                {
                    bool check = true;
                    foreach (var ite in allDetails)
                    {
                        if (item.Name == ite.Name)
                        {
                            check = false;
                        }
                    }
                    if (check)
                    {
                        union.Add(item);
                    }
                }
                allDetails.AddRange(union);
                return (allDetails);
            }
        }
    }
}