using Grand.Core;
using Grand.Core.Caching;
using Grand.Domain.Media;
using Grand.Domain.News;
using Grand.Services.Helpers;
using Grand.Services.Localization;
using Grand.Services.Media;
using Grand.Services.News;
using Grand.Services.Seo;
using Grand.Web.Features.Models.News;
using Grand.Web.Infrastructure.Cache;
using Grand.Web.Models.Media;
using Grand.Web.Models.News;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Grand.Web.Features.Handlers.News
{
    public class GetHomePageNewsItemsHandler : IRequestHandler<GetHomePageNewsItems, HomePageNewsItemsModel>
    {
        private readonly ICacheManager _cacheManager;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly INewsService _newsService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;

        private readonly NewsSettings _newsSettings;
        private readonly MediaSettings _mediaSettings;

        public GetHomePageNewsItemsHandler(ICacheManager cacheManager, IWorkContext workContext, IStoreContext storeContext,
            INewsService newsService, IDateTimeHelper dateTimeHelper, IPictureService pictureService, 
            ILocalizationService localizationService, NewsSettings newsSettings, MediaSettings mediaSettings)
        {
            _cacheManager = cacheManager;
            _workContext = workContext;
            _storeContext = storeContext;
            _newsService = newsService;
            _dateTimeHelper = dateTimeHelper;
            _pictureService = pictureService;
            _localizationService = localizationService;
            _newsSettings = newsSettings;
            _mediaSettings = mediaSettings;
        }

        public async Task<HomePageNewsItemsModel> Handle(GetHomePageNewsItems request, CancellationToken cancellationToken)
        {
            var cacheKey = string.Format(ModelCacheEventConst.HOMEPAGE_NEWSMODEL_KEY, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id);
            var model = await _cacheManager.GetAsync(cacheKey, async () =>
            {
                var newsItems = await _newsService.GetAllNews(_storeContext.CurrentStore.Id, 0, _newsSettings.MainPageNewsCount);
                var hpnitemodel = new HomePageNewsItemsModel();
                foreach (var item in newsItems)
                {
                    var newsModel = await PrepareNewsItemModel(item);
                    hpnitemodel.NewsItems.Add(newsModel);
                }
                return hpnitemodel;
            });

            return model;
        }

        private async Task<HomePageNewsItemsModel.NewsItemModel> PrepareNewsItemModel(NewsItem newsItem)
        {
            var model = new HomePageNewsItemsModel.NewsItemModel();
            model.Id = newsItem.Id;
            model.SeName = newsItem.GetSeName(_workContext.WorkingLanguage.Id);
            model.Title = newsItem.GetLocalized(x => x.Title, _workContext.WorkingLanguage.Id);
            model.Short = newsItem.GetLocalized(x => x.Short, _workContext.WorkingLanguage.Id);
            model.Full = newsItem.GetLocalized(x => x.Full, _workContext.WorkingLanguage.Id);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(newsItem.StartDateUtc ?? newsItem.CreatedOnUtc, DateTimeKind.Utc);
            //prepare picture model
            if (!string.IsNullOrEmpty(newsItem.PictureId))
            {
                int pictureSize = _mediaSettings.NewsListThumbPictureSize;
                var categoryPictureCacheKey = string.Format(ModelCacheEventConst.NEWS_PICTURE_MODEL_KEY, newsItem.Id, pictureSize, true,
                    _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id);
                model.PictureModel = await _cacheManager.GetAsync(categoryPictureCacheKey, async () =>
                {
                    var pictureModel = new PictureModel {
                        Id = newsItem.PictureId,
                        FullSizeImageUrl = await _pictureService.GetPictureUrl(newsItem.PictureId),
                        ImageUrl = await _pictureService.GetPictureUrl(newsItem.PictureId, pictureSize),
                        Title = string.Format(_localizationService.GetResource("Media.News.ImageLinkTitleFormat"), newsItem.Title),
                        AlternateText = string.Format(_localizationService.GetResource("Media.News.ImageAlternateTextFormat"), newsItem.Title)
                    };
                    return pictureModel;
                });
            }
            return model;
        }
    }
}
