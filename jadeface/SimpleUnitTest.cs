using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace jadeface
{
    [TestClass]

    public class SimpleUnitTest
    {
        //[TestMethod]
        //public void BookListItemTest()                  //POJO测试
        //{
        //    BookListItem item = new BookListItem();

        //    item.Author = "test";
        //    Assert.IsTrue(item.Author.Equals("test"));

        //    item.CurPageNo = 100;
        //    Assert.IsTrue(item.CurPageNo == 100);
        //}

        [TestMethod]
        public void BookServiceInsertBoundaryTest()                    //BookService边界测试
        {
            BookService service = BookService.getInstance();

            BookListItem book = new BookListItem();     //边界测试需要的数据

            bool ret = service.insert(book);            //边界测试1，插入，Book内容为空
            Assert.IsTrue(ret == false);

        }

        [TestMethod]
        public void BookServiceSearchBoundaryTest()                    //BookService边界测试
        {
            BookService service = BookService.getInstance();


            BookListItem book = new BookListItem();     //边界测试需要的数据
            List<BookListItem> list;

            list = service.searchByISBN(book.ISBN, "");
            Assert.IsTrue(list == null);                //边界测试2，查询，username为空

        }

        [TestMethod]
        public void BookServiceDeleteBoundaryTest()                    //BookService边界测试
        {
            BookService service = BookService.getInstance();


            BookListItem book = new BookListItem();     //边界测试需要的数据

            bool ret2 = service.delete(book, "");       //边界测试3，删除，username为空
            Assert.IsTrue(ret2 == false);

        }

        [TestMethod]
        public void BookServiceUpdateBoundaryTest()                    //BookService边界测试
        {
            BookService service = BookService.getInstance();

            BookListItem book = new BookListItem();     //边界测试需要的数据

            bool ret3 = service.update(book);           //边界测试4，更新，Book内容为空
            Assert.IsTrue(ret3 == false);

        }

        //[TestMethod]
        //public void BookServiceInsertFuncTest()               //BookService功能测试
        //{
        //    BookService service = BookService.getInstance();

        //    BookListItem book2 = new BookListItem();    //正常测试需要的数据
        //    book2.UserId = "TestUser";
        //    book2.Title = "TestBook";
        //    book2.PageNo = 100;
        //    book2.CurPageNo = 0;
        //    book2.Id = 0;
        //    book2.Publisher = "TestPublisher";
        //    book2.Author = "TestAuthor";
        //    book2.ISBN = "10010";

        //    bool ret4 = service.insert(book2);                   //正常测试1，插入
        //    Assert.IsTrue(ret4 == true);
        //}

        //[TestMethod]
        //public void BookServiceSearchFuncTest()               //BookService功能测试
        //{
        //    BookService service = BookService.getInstance();

        //    List<BookListItem> list;

        //    BookListItem book2 = new BookListItem();    //正常测试需要的数据
        //    book2.UserId = "TestUser";
        //    book2.Title = "TestBook";
        //    book2.PageNo = 100;
        //    book2.CurPageNo = 0;
        //    book2.Id = 0;
        //    book2.Publisher = "TestPublisher";
        //    book2.Author = "TestAuthor";
        //    book2.ISBN = "10010";

        //    list = service.searchByISBN(book2.ISBN, "TestUser");
        //    Assert.IsTrue(list.Count == 1);                      //正常测试2，查询

        //}

        [TestMethod]
        public void BookServiceUpdateFuncTest()               //BookService功能测试
        {
            BookService service = BookService.getInstance();
            List<BookListItem> list;
            list = service.searchByISBN("10010", "TestUser");    //正常测试需要的数据
            BookListItem book2 = list[0];
            book2.Author = "TestAuthorx";

            bool ret5 = service.update(book2);                   //正常测试3，更新
            Assert.IsTrue(ret5 == true);
        }

        //[TestMethod]
        //public void BookServiceDeleteFuncTest()               //BookService功能测试
        //{
        //    BookService service = BookService.getInstance();

        //    BookListItem book2 = new BookListItem();    //正常测试需要的数据
        //    book2.UserId = "TestUser";
        //    book2.Title = "TestBook";
        //    book2.PageNo = 100;
        //    book2.CurPageNo = 0;
        //    book2.Id = 0;
        //    book2.Publisher = "TestPublisher";
        //    book2.Author = "TestAuthor";
        //    book2.ISBN = "10010";

        //    bool ret6 = service.delete(book2, "TestUser");       //正常测试4，删除
        //    Assert.IsTrue(ret6 == true);
        //}

        [TestMethod]
        public void RefreshWishBookListBoundaryTest()                             //刷新愿望书单边界测试
        {
            BookService service = BookService.getInstance();
            List<BookListItem> list = service.RefreshWishBookList("");
            Assert.IsTrue(list == null);
        }

        [TestMethod]
        public void RefreshBookListBoundaryTest()                                  //刷新在读书单边界测试
        {
            BookService service = BookService.getInstance();
            List<BookListItem> list = service.RefreshBookList("");
            Assert.IsTrue(list == null);
        }

        [TestMethod]
        public void RefreshFinishBookListBoundaryTest()                            //刷新完成书单边界测试
        {
            BookService service = BookService.getInstance();
            List<BookListItem> list = service.RefreshFinishBookList("");
            Assert.IsTrue(list == null);
        }

        [TestMethod]
        public void RefreshWishBookListFuncTest()                             //刷新愿望书单功能测试
        {
            BookService service = BookService.getInstance();
            List<BookListItem> list = service.RefreshWishBookList("TestUser");
            Assert.IsTrue(list != null);
        }

        [TestMethod]
        public void RefreshBookListFuncTest()                                  //刷新在读书单功能测试
        {
            BookService service = BookService.getInstance();
            List<BookListItem> list = service.RefreshBookList("TestUser");
            Assert.IsTrue(list != null);
        }

        [TestMethod]
        public void RefreshFinishBookListFuncTest()                            //刷新完成书单功能测试
        {
            BookService service = BookService.getInstance();
            List<BookListItem> list = service.RefreshFinishBookList("TestUser");
            Assert.IsTrue(list != null);
        }

    }
}
