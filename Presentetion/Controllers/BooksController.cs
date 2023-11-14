﻿using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Presentetion.ActionFilters;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Presentetion.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [ApiController]
    [Route("api/Books")]
    
        public class BooksController : ControllerBase
        {

            private readonly IServiceManager _manager;

            public BooksController(IServiceManager manager)
            {
                _manager = manager;
            }

            [HttpGet]
            [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
            public async Task<IActionResult> GetAllBooksAsync([FromQuery]BookParameters bookParameters)
            {
            var linkParameters = new LinkParameters()
            {
                BookParameters = bookParameters,
                HttpContext = HttpContext
            };
                    var result = await _manager
                        .BookService
                        .GettAllBooksAsync(linkParameters, false);
                    
                    Response.Headers.Add("X-Pagination", 
                        JsonSerializer.Serialize(result.metaData));

                    return result.linkResponse.HasLinks ? 
                Ok(result.linkResponse.LinkedEntities) :
                Ok(result.linkResponse.ShapedEntities);
            }
            [HttpGet("{id:int}")]
            public async Task<IActionResult> GetOneBookAsync([FromRoute(Name = "id")] int id)
            {
                    var book = await _manager.BookService
                        .GetOneBookByIdAsync(id, false);

                    
                    return Ok(book);
            }
            [ServiceFilter(typeof(ValidationFilterAttribute))]
            [HttpPost]//create
            public async Task<IActionResult> CreateOneBookAsync([FromBody] BookDtoForInsertion bookDto)
            {
                 var book = await _manager.BookService.CreateOneBookAsync(bookDto);
                 return StatusCode(201, book);//CreateAtRoute()
            }
            [ServiceFilter(typeof(ValidationFilterAttribute))]
            [HttpPut("{id:int}")]
            public async Task<IActionResult> UpdateOneBookAsync([FromRoute(Name = "id")] int id,
                [FromBody] BookDtoForUpdate bookDto)
            {
            await _manager.BookService.UpdateOneBookAsync(id, bookDto, false);
                    return NoContent();//204
            }
            [HttpDelete("{id:int}")]
            public async Task<IActionResult> DeleteOneBookAsync([FromRoute(Name = "id")] int id)
            {
                    await _manager.BookService.DeleteOneBookAsync(id, false);
                    return NoContent();
            }

            [HttpPatch("{id:int}")]
            public async Task<IActionResult> PartiallyUpdateOneBookAsync([FromRoute(Name = "id")] int id,
                [FromBody] JsonPatchDocument<BookDtoForUpdate> bookPatch)
            {
                    if(bookPatch is null)
                       return BadRequest();
                    var result = await _manager.BookService.GetOneBookForPatchAsync(id, false);

                    bookPatch.ApplyTo(result.bookDtoForUpdate, ModelState);
                        
                    TryValidateModel(result.bookDtoForUpdate);

                    if (!ModelState.IsValid)
                    {
                     return UnprocessableEntity(ModelState);
                    }

                   await _manager.BookService.SaveChangesForPatchAsync(result.bookDtoForUpdate, result.book);
                    return NoContent(); //204
            }
        }
    
}
