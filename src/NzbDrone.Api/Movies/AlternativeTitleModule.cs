using System;
using System.Collections.Generic;
using System.Linq;
using Marr.Data;
using Nancy;
using NzbDrone.Api;
using NzbDrone.Api.Movie;
using NzbDrone.Common.Cache;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.MediaCover;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.MediaFiles.EpisodeImport;
using NzbDrone.Core.MetadataSource;
using NzbDrone.Core.MetadataSource.RadarrAPI;
using NzbDrone.Core.Movies.AlternativeTitles;
using NzbDrone.Core.RootFolders;
using NzbDrone.Core.Tv;

namespace NzbDrone.Api.Movie
{
    public class AlternativeTitleModule : NzbDroneRestModule<AlternativeTitleResource>
    {
        private readonly IAlternativeTitleService _altTitleService;
        private readonly IMovieService _movieService;
        private readonly IRadarrAPIClient _radarrApi;

        public AlternativeTitleModule(IAlternativeTitleService altTitleService, IMovieService movieService, IRadarrAPIClient radarrApi)
            : base("/alttitle")
        {
            _altTitleService = altTitleService;
            _movieService = movieService;
            _radarrApi = radarrApi;
            CreateResource = AddTitle;
            GetResourceById = GetTitle;
        }

        private int AddTitle(AlternativeTitleResource altTitle)
        {
            var title = altTitle.ToModel();
            var movie = _movieService.GetMovie(altTitle.MovieId);
            var newTitle = _radarrApi.AddNewAlternativeTitle(title, movie.TmdbId);

            var addedTitle = _altTitleService.AddAltTitle(newTitle, movie);
            return addedTitle.Id;
        }

        private AlternativeTitleResource GetTitle(int id)
        {
            return _altTitleService.GetById(id).ToResource();
        }
    }
}