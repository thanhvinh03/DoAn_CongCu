using AutoMapper;
using FoodStore.DTO;
using FoodStore.Models;
using FoodStore.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FoodStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuffetController : ControllerBase
    {
        private readonly IBuffetRepository _buffetRepository;
        private readonly IBuffetDetailRepository _buffetDetailRepository;
        private readonly IFoodRepository _foodRepository;
        private readonly IMapper _mapper;

        public BuffetController(IBuffetRepository buffetRepository, IBuffetDetailRepository buffetDetailRepository, IFoodRepository foodRepository, IMapper mapper)
        {
            _buffetRepository = buffetRepository;
            _buffetDetailRepository = buffetDetailRepository;
            _foodRepository = foodRepository;
            _mapper = mapper;
        }

        // GET: api/Buffet
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BuffetDTO>>> GetAllBuffets()
        {
            var buffets = await _buffetRepository.GetAllBuffetsAsync();
            return Ok(_mapper.Map<List<BuffetDTO>>(buffets));
        }

        // GET: api/Buffet/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBuffetById(int id)
        {
            var buffet = await _buffetRepository.GetBuffetByIdAsync(id);
            if (buffet == null)
            {
                return NotFound();
            }

            var buffetDetails = await _buffetDetailRepository.GetAllBuffetDetailsAsync(id);
            var buffetDTO = _mapper.Map<BuffetDTO>(buffet);
            return Ok(buffetDTO);
        }

        // POST: api/Buffet
        [HttpPost]
        public async Task<IActionResult> CreateBuffet(BuffetDTO buffetDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var buffet = _mapper.Map<Buffet>(buffetDTO);
            await _buffetRepository.CreateBuffetAsync(buffet);
            return CreatedAtAction(nameof(GetBuffetById), new { id = buffet.Id }, buffet);
        }

        // PUT: api/Buffet/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBuffet(int id, BuffetDTO buffetUpdateDTO)
        {
            if (id != buffetUpdateDTO.Id)
            {
                return BadRequest("ID in URL does not match ID in model.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingBuffet = await _buffetRepository.GetBuffetByIdAsync(id);
            if (existingBuffet == null)
            {
                return NotFound();
            }

            existingBuffet.Status = buffetUpdateDTO.Status;
            existingBuffet.Price = buffetUpdateDTO.Price;
            existingBuffet.Description = buffetUpdateDTO.Description;
            existingBuffet.Name = buffetUpdateDTO.Name;
            existingBuffet.Image = buffetUpdateDTO.Image;

            await _buffetRepository.UpdateBuffetAsync(existingBuffet);
            return Ok(existingBuffet);
        }

        // DELETE: api/Buffet/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBuffet(int id)
        {
            var buffet = await _buffetRepository.GetBuffetByIdAsync(id);
            if (buffet == null)
            {
                return NotFound();
            }

            await _buffetRepository.DeleteBuffetAsync(id);

            return Ok();
        }
    }
}
