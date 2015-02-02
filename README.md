InstagramFeed
=============
Description : Web API component that provides images from Instagram based on hashtags.
Author : Shukri Adams (shukri.adams@gmail.com)
Date: December, 2014

Requirements
------------
- ASP Web API site running .Net 4.5
- Some kind IOC framework to connect the persistence classes with the core logic (We like SimpleInjector).

Setup
-----
* Create an MVC or Web API project. To this add the InstagramFeed project.
* Also add a data persistence layer (InstagramParse for Parse.com, InstagramAzure for Azure storage).
* In your web api site project, register the persistence classes for IInstagramImages and IImageVotes with your IOC framework of choice. (see Global.asax.cs in the included Site test project for examples using the SimpleInjector IOC framework).
* In your web api site project, add the following to your web.config (see the included Site test project for details) :
	
	<configuration>  
	  <configSections>
	    <section name="instagramFeedSettings" type="InstagramFeed.InstagramFeedSettingsHandler, InstagramFeed" />
	    <section name="instagramFeedParseSettings" type="InstagramFeed.Parse.ParseSettingsHandler, InstagramFeed.Parse" />
	  </configSections>

	  <instagramFeedSettings 
	    instagramClientId="yourInstagramAppId" 
	    hashTags="tagToSearchBy,anOtionalOtherTagToLimitBy" 
	    pageSize="3"
	    allowedOrigins="http://my.authorizedDomain.com,http://my.otherAuthorizedDomain.com"
	    pollInterally="false"
	    startDate="20140101"
	    adminCookieKey="someReallyHardToGuessString" 
	    singleVotePerUser="false" 
	    instagramPollInterval="30" />
	
	  <instagramFeedParseSettings
	    parseAppId="yourParseAppId" 
	    parseRestApiKey="yourParseRestApiKey"  />	    

    </configuration>

  
  The config section values are :

	  <instagramFeedSettings 

	    instagramClientId="REQUIRED STRING. The Instagram API client ID you will be querying with." 
	  
	    hashTags="REQUIRED STRING. The hash tag to search Instragram with." 
	  
	    pageSize="OPTIONAL INT. The number of images per page when querying the InstagramFeed API. Default is 12."
	  
	    allowedOrigins="OPTIONAL STRING. Comma separated list of IPs which will be allowed to query this API. Blank allows all."
	  
	    pollInterally="OPTIONAL BOOL. If true, requesting images from this API will cause Instagram to be periodically queried. If false, this API's PollInstagram method must be explicitly called whenever Instagram must be queried, and no throttle limit is performed. Use this feature if you want to manually control when Instagram is queried. Default is true."
	  
	    startDate="OPTIONAL INT. ISO short date (yyymmdd) for date at which API will begin to poll Instagram."
	  
	    adminKey="OPTIONAL STRING. If cookie or request header with this key is present, request will be allowed to perform admin functions." 
	  
	    singleVotePerUser="OPTIONAL BOOL. True limits use to vote for on Instagram image only." 
	  
	    instagramPollInterval="OPTIONAL INT. Soft throttle for minimum elapsed time (in seconds) between calls to Instagram API, meaning.  Default is 60." />
	
	  <!-- Required if using InstagramParse persistence layer -->  
	  <instagramFeedParseSettings
	    parseAppId="REQUIRED STRING, Parse.com app id" 
	    parseRestApiKey="REQUIRED STRING, Parse.com REST app key" />	    

	  <!-- Required if using InstagramAzure persistence layer -->  
	  <instagramFeedParseSettings
	    parseAppId="REQUIRED STRING" 
	    parseRestApiKey="REQUIRED STRING" />	    



* In your web api site project, make sure InstagramController is registered as an API endpoint. This will normally happen automatically, and the default url will be http:yoursite/api/instagram. You
  can override your project's route register to set your own endpoint address of course.
* You need a valid Instagram.
* Start your site, and navigate to /api/instagram/getImages. This should return a list of images, either as XML or JSON.




