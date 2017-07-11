namespace Tinifier.Core.Infrastructure
{
    public class PackageConstants
    {
        public const int MonthlyRequestsLimit = 500;
        public const string TinyPngUrl = "https://api.tinify.com";
        // public const string BackEndDevsUrl = "http://backend-devs.com";
        public const string BackEndDevsUrl = "http://localhost:53151/";

        public const string TinyPngUri = "/shrink";
        public const string FirstNodeId = "-1";
        public const string SectionAlias = "tinifier";
        public const string SectionName = "Tinifier";
        public const string SectionIcon = "icon-wand";
        public const string MenuIcon = "umb-settings";
        public const string MenuSettingsIcon = "article";
        public const string MenuIconBulk = "lightbulb-active";
        public const string ImageAlias = "image";
        public const string MediaAlias = "media";
        public const string TabAlias = "tab";
        public const string AreaAlias = "area";
        public const string AreasAlias = "areas";
        public const string ControlAlias = "control";
        public const string CaptionAlias = "caption";
        public const string FolderAlias = "folder";
        public const string Alias = "alias";
        public const string Section = "section";
        public const string Src = "src";
        public const string TinifierButton = "Tinifier_Button";
        public const string TinifierButtonCaption = "Tinify";
        public const string StatsButton = "Tinifier_Settings";
        public const string StatsButtonCaption = "Stats";
        public const string StatsDialogCaption = "Optimization Stats";
        public const string ErrorCategory = "Validation";
        public const string TreeAlias = "timages";
        public const string TreeTitle = "Optimized Images";
        public const string TreeIcon = "icon-umb-media";
        public const string TinifierSettings = "TinifierSettings";
        public const string Settings = "Settings";
        public const string Statistic = "Statistic";
        public const string ApiKey = "api:";
        public const string UmbracoFileAlias = "umbracoFile";
        public const string PngExtension = ".png";
        public const string JpgExtension = ".jpg";
        public const string JpeExtension = ".jpe";
        public const string JpegExtension = ".jpeg";
        public const string BasicAuth = "Basic";
        public const string ContentTypeHeader = "Content-Type";
        public const string ContentType = "application/json";
        public const string TinyPngHeader = "Compression-Count";

        public const string DbSettingsTable = "TinifierUserSettings";
        public const string DbHistoryTable = "TinifierResponseHistory";
        public const string DbStatisticTable = "TinifierImagesStatistic";
        public const string DbStateTable = "TinifierState";

        public const string TinyTImageRoute = "/App_Plugins/Tinifier/BackOffice/timages/tinifyDialog.html";
        public const string TinySettingsRoute = "/App_Plugins/Tinifier/BackOffice/timages/edit.html";
        public const string SettingsTabRoute = "/App_Plugins/Tinifier/BackOffice/Dashboards/settings.html";
        public const string StatisticTabRoute = "/App_Plugins/Tinifier/BackOffice/Dashboards/statistic.html";
        public const string BackEndDevsPostStatistic = "/umbraco/api/Tinifier/PostStatistic";
        public const string PathToDashboard = "~/config/dashboard.config";
        public const string PathToSectionDashboard = "//section[@alias='TinifierSettings']";

        public const string BadRequest = "Bad request";
        public const string ApiKeyMessage = "ApiKey successfully added!";
        public const string ApiKeyError = "ApiKey not added! Please, fill ApiKey field with correct value";
        public const string AlreadyOptimized = "Image is already optimized";
        public const string SuccessOptimized = "Image(s) succesfully optimized!";
        public const string NotSupported = "This extension not supported. You can tinify only PNG and JPG files";
        public const string ApiKeyNotFound = "You don`t have ApiKey in settings! Please, go to tinifier section and add ApiKey there!";
        public const string TooBigImage = "The request was timed out! Your image is too big!";
        public const string AllImagesAlreadyOptimized = "All Images are already optimized";
        public const string ConcurrentOptimizing = "Sorry, but at the same time you can tinify only one Image";
        public const string NotSuccessfullRequest = "Request to TinyPNG was not successfull";
        public const string ImageNotExists = "Image with such id doesn't exist. Id: ";
        public const string ImageWithPathNotExists = "Image with such path doesn't exist. Name: ";
        public const string NotAllImagesWereOptimized = "Not all images have been optimized";
    }
}
