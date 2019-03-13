using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WM.GUID.Application.Commands.CreateGUID;
using WM.GUID.Application.Commands.UpdateGUID;
using WM.GUID.Application.Commands.DeleteGUID;
using WM.GUID.Application.Queries.ReadGUID;
using System;
using WM.GUID.Application.Exceptions;

namespace WM.GUID.WebAPI
{
    /// <summary>
    /// CRUD application for GUID Metadata
    /// </summary>
    [Produces("application/json")]
    [Route("guid")]
    public class GUIDController : Controller
    {
        private readonly ILogger<GUIDController> _logger;
        private readonly IMediator _mediator;

        /// <summary>
        /// GUID Controller constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mediator"></param>
        public GUIDController(ILogger<GUIDController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        ///     Get a GUID metadata record
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     GET/guid
        ///     Id: 51164976C388410FA47202A11FFF4C2B   
        /// </remarks>
        /// <param name="query">GUID</param>
        /// <response code="200">Returns a GUID DTO</response>
        /// <response code="404">Not Found</response>
        /// <response code="410">Gone</response>/// 
        /// <response code="500">Returns a result ErrorInfo describing Internal Error</response>/// 
        /// <returns>GuidDTO</returns>
        [HttpGet]
        [ProducesResponseType(typeof(GuidDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status410Gone)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGUID(GetGUIDQuery query)
        {
            try
            {
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Debug, ex.Message);

                if (ex is InvalidOperationException)
                    return NotFound();
                else if (ex is ExpiredException)
                    return StatusCode(410);
            }
            return StatusCode(500);
        }

        /// <summary>
        ///     Creates a GUID Metadata record with a generated Key
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     POST/guid
        ///     body:
        ///     {
        ///     "user":"John Doe",
        ///     }
        ///     *****************************************
        ///     Sample request:
        ///     POST/guid
        ///     body:
        ///     {
        ///     "user":"John Doe",
        ///     "expire":9999999
        ///     }
        /// </remarks>
        /// <param name="createCommand">Create GUID Metadata command</param>
        /// <returns>GuidDTO</returns>
        /// <response code="200">Returns a GUID DTO</response>
        /// <response code="500">Returns a result ErrorInfo describing Internal Error</response>
        [HttpPost]
        [ProducesResponseType(typeof(GuidDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateGUIDCommand createCommand)
        {
            try
            {
                var dto = await _mediator.Send(createCommand);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Debug, ex.Message);

                if (ex is ArgumentNullException || ex is FormatException)
                    return StatusCode(400);
            }
            return StatusCode(500);
        }

        /// <summary>
        ///     Creates a GUID Metadata record with an existing Key
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     POST/guid
        ///     Id: 51164976C388410FA47202A11FFF4C2B
        ///     body:
        ///     {
        ///     "id":"51164976C388410FA47202A11FFF4C2B",
        ///     "user":"John Doe",
        ///     "expire":9999999999
        ///     }
        /// </remarks>
        /// <param name="createCommand">Create GUID Metadata command</param>
        /// <param name="key">GUID key</param>/// 
        /// <returns>GuidDTO</returns>
        /// <response code="200">Returns a GUID DTO</response>
        /// <response code="400">Not Found</response>
        /// <response code="500">Returns a result ErrorInfo describing Internal Error</response>
        [HttpPost("{key}", Name = "GUID")]
        [ProducesResponseType(typeof(GuidDTO), StatusCodes.Status200OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromRoute] string key, [FromBody] CreateGUIDCommand createCommand)
        {
            createCommand.Id = key;
            try
            {
                var dto = await _mediator.Send(createCommand);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Debug, ex.Message);

                if (ex is ArgumentNullException || ex is FormatException)
                    return StatusCode(400);
            }
            return StatusCode(500);
        }

        /// <summary>
        ///     Update a Guid Metadata record
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     PUT/guid
        ///     Id: 51164976C388410FA47202A11FFF4C2B
        ///     body:
        ///     {
        ///     "expire":"1000000000",
        ///     "user":"Jane Doe",
        ///     }
        ///     ****************************************
        ///     {
        ///     "expire":"9999999999",
        ///     "isDeleted":false
        ///     }
        ///     ****************************************
        ///     {
        ///     "user":"John Doe",
        ///     }
        ///     ****************************************
        ///     {
        ///     "isDeleted":false
        ///     }
        /// </remarks>
        /// <param name="updateCommand">Update GUID metadata command</param>
        /// <param name="key">GUID key</param>/// 
        /// <response code="200">Returns a GUID DTO</response>
        /// <response code="400">Returns a result with Errors describing invalid state</response>
        /// <response code="500">Returns a result ErrorInfo describing Internal Error</response>
        /// <returns>GuidDTO</returns>
        [HttpPut("{key}", Name = "GUID")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> Update([FromRoute] string key, [FromBody] UpdateGUIDCommand updateCommand)
        {
            updateCommand.Id = key;
            try
            {
                var commandResult = await _mediator.Send(updateCommand);
                return Ok(commandResult);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Debug, ex.Message);

                if (ex is ArgumentNullException || ex is FormatException)
                    return StatusCode(400);
                else if (ex is InvalidOperationException)
                    return NotFound();
            }
            return StatusCode(500);

        }

        /// <summary>
        ///     Delete a GUID metadata record
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     Delete/GUID
        ///     Id: 51164976C388410FA47202A11FFF4C2B
        ///     body:
        ///     {
        ///     "id":"51164976C388410FA47202A11FFF4C2B",
        ///     }     
        /// </remarks>
        /// <param name="id">Delete GUID metadata Id</param>
        /// <response code="204">No Content</response>
        /// <response code="400">Returns a result with Errors describing invalid state</response>
        /// <response code="500">Returns a result ErrorInfo describing Internal Error</response>
        /// <returns>No Content</returns>
        [HttpDelete("{Key}", Name = "GUID")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                await _mediator.Send(new DeleteGUIDCommand { Id = id } );
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Debug, ex.Message);

                if (ex is InvalidOperationException)
                    return NotFound();
            }
            return StatusCode(500);
        }
    }
}