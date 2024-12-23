using AutoMapper;
using FoodStore.DTO;
using FoodStore.Models;
using FoodStore.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuffetDetailController : ControllerBase
    {
        private readonly IBuffetDetailRepository _buffetDetailRepository;
        private readonly IMapper _mapper;

        public BuffetDetailController(IBuffetDetailRepository buffetDetailRepository, IMapper mapper)
        {
            _buffetDetailRepository = buffetDetailRepository;
            _mapper = mapper;
        }

        // GET: api/BuffetDetailByIdBuffet
        [HttpGet("/api/BuffetDetailByIdBuffet")]
        public async Task<ActionResult<IEnumerable<BuffetDetailDTO>>> GetAllBuffetDetails(int idBuffet)
        {
            var buffetDetails = await _buffetDetailRepository.GetAllBuffetDetailsAsync(idBuffet);
            return Ok(_mapper.Map<List<BuffetDetailDTO>>(buffetDetails));
        }

        // GET: api/BuffetDetail/5
        [HttpGet("{buffetId}")]
        public async Task<ActionResult<IEnumerable<BuffetDetailDTO>>> GetBuffetDetailsByBuffetId(int buffetId)
        {
            var buffetDetails = await _buffetDetailRepository.GetAllBuffetDetailsAsync(buffetId);
            if (buffetDetails == null || !buffetDetails.Any())
            {
                return NotFound();
            }

            return Ok(_mapper.Map<List<BuffetDetailDTO>>(buffetDetails));
        }

        // POST: api/BuffetDetail
        [HttpPost]
        public async Task<IActionResult> AddBuffetDetail(BuffetDetailDTO buffetDetailCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var buffetDetail = _mapper.Map<BuffetDetail>(buffetDetailCreateDTO);
            await _buffetDetailRepository.CreateBuffetDetailAsync(buffetDetail);

            return CreatedAtAction(nameof(GetBuffetDetailsByBuffetId), new { buffetId = buffetDetail.BuffetId }, buffetDetail);
        }

        // DELETE: api/BuffetDetail
        [HttpDelete]
        public async Task<IActionResult> RemoveBuffetDetail(int id)
        {
            try
            {
                await _buffetDetailRepository.DeleteBuffetDetailAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            
        }
    }
}
