using Entities.DataTransferObjects;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BookLinks : IBookLinks
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IDataShaper<BookDto> _dataShaper;

        public BookLinks(LinkGenerator linkGenerator, IDataShaper<BookDto> dataShaper)
        {
            _linkGenerator = linkGenerator;
            _dataShaper = dataShaper;
        }

        public LinkResponse TryGenerateLinks(IEnumerable<BookDto> booksDto, string fields, HttpContext httpContext)
        {
            var shapedBook = ShapeData(booksDto, fields);
            if (ShouldGenerateLinks(httpContext))
            {
                return ReturnLinkedBooks(booksDto, fields,httpContext, shapedBook);
            }
            return ReturnShapedBooks(shapedBook);
        }

        private LinkResponse ReturnLinkedBooks(IEnumerable<BookDto> booksDto,
            string fields,
            HttpContext httpContext,
            List<Entity> shapedBook)
        {
            var bookDtoList = booksDto.ToList();
            for (int index = 0;index < bookDtoList.Count(); index++)
            {
                var bookLinks = CreateForBook(httpContext, bookDtoList[index], fields);
                shapedBook[index].Add("Links", bookLinks);
            }
            var bookCollection = new LinkCollectionWrapper<Entity>(shapedBook);
            return new LinkResponse { HasLinks = true, LinkedEntities = bookCollection };
        }

        private List<Link> CreateForBook(HttpContext httpContext, BookDto bookDto, string fields)
        {
            var links = new List<Link>()
            {
                new Link("a1", "b1", "c1"),
                new Link("a2", "b2", "c2")
            };
            return links;
        }

        private LinkResponse ReturnShapedBooks(List<Entity> shapedBook)
        {
            return new LinkResponse() { ShapedEntities = shapedBook };
        }

        private bool ShouldGenerateLinks(HttpContext httpContext)
        {
            var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];
            return mediaType
                .SubTypeWithoutSuffix
                .EndsWith("hateoas", StringComparison.OrdinalIgnoreCase);
        }

        private List<Entity> ShapeData(IEnumerable<BookDto> booksDto, string fields)
        {
            return _dataShaper.ShapeData(booksDto, fields)
                .Select(b => b.Entity)
                .ToList();
        }
    }
}
