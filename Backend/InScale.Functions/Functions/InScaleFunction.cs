namespace InScale.Functions.Functions
{
    using InScale.Contracts.Settings;
    using InScale.Contracts.Storage;
    using MediatR;

    public partial class InScaleFunction
    {
        private readonly IMediator _mediator;
        private readonly IStorageService _storageService;
        private readonly IStorageSettings _storageSettings;

        public InScaleFunction(IMediator mediator, IStorageService storageService, IStorageSettings storageSettings)
        {
            _mediator = mediator;
            _storageService = storageService;
            _storageSettings = storageSettings;
        }
    }
}
