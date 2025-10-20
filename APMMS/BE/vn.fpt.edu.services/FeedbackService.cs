using AutoMapper;
using BE.vn.fpt.edu.DTOs.Feedback;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;

namespace BE.vn.fpt.edu.services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _repository;
        private readonly IMapper _mapper;

        public FeedbackService(IFeedbackRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ResponseDto>> GetAllAsync()
        {
            var feedbacks = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ResponseDto>>(feedbacks);
        }

        public async Task<ResponseDto?> GetByIdAsync(long id)
        {
            var feedback = await _repository.GetByIdAsync(id);
            return _mapper.Map<ResponseDto?>(feedback);
        }

        public async Task<IEnumerable<ResponseDto>> GetByUserIdAsync(long userId)
        {
            var feedbacks = await _repository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<ResponseDto>>(feedbacks);
        }

        public async Task<IEnumerable<ResponseDto>> GetByTicketIdAsync(long ticketId)
        {
            var feedbacks = await _repository.GetByTicketIdAsync(ticketId);
            return _mapper.Map<IEnumerable<ResponseDto>>(feedbacks);
        }

        public async Task<ResponseDto> CreateAsync(RequestDto request)
        {
            var feedback = _mapper.Map<Feedback>(request);
            var created = await _repository.CreateAsync(feedback);
            return _mapper.Map<ResponseDto>(created);
        }

        public async Task<ResponseDto?> UpdateAsync(long id, RequestDto request)
        {
            var feedback = _mapper.Map<Feedback>(request);
            feedback.Id = id;
            var updated = await _repository.UpdateAsync(feedback);
            return _mapper.Map<ResponseDto?>(updated);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
