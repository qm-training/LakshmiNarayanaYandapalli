namespace WelfareWorkTracker.Infrastructure.Service;
public class ConstituencyService(IConstituencyRepository constituencyRepository,
                                    IMapper mapper) : IConstituencyService
{
    private readonly IConstituencyRepository _constituencyRepository = constituencyRepository;
    private readonly IMapper _mapper = mapper;
    public async Task<ConstituencyDto> AddConstituencyAsync(ConstituencyVm constituencyVm)
    {
        var constituency = new Constituency
        {
            ConstituencyName = constituencyVm.ConstituencyName,
            DistrictName = constituencyVm.DistrictName,
            StateName = constituencyVm.StateName,
            CountryName = constituencyVm.CountryName,
            Pincode = constituencyVm.Pincode
        };

        var addedConstituency = await _constituencyRepository.AddConstituencyAsync(constituency);
        var constituencyDto = _mapper.Map<ConstituencyDto>(addedConstituency);
        return constituencyDto;
    }

    public async Task<List<ConstituencyDto>> GetAllConstituenciesAsync()
    {
        var constituencies = await _constituencyRepository.GetConstituenciesAsync();
        if (constituencies == null || constituencies.Count == 0)
            throw new WelfareWorkTrackerException("No constituency found", (int)HttpStatusCode.NotFound);

        var constituencyDtos = new List<ConstituencyDto>();
        foreach (var constituency in constituencies)
        {
            var constituencyDto = _mapper.Map<ConstituencyDto>(constituency);
            constituencyDtos.Add(constituencyDto);
        }

        return constituencyDtos;
    }
}