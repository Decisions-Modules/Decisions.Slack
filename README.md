# Decisions.Slack Integration
  Slack uses two client type: _Bot_ and _User_. And there are two _AccessTokens_ for them.
_Bot_'s abilities are more limited compared with _User_. _Bots_ cannot use some API methods like _Search_ and _Create Channel_. So it’s better to get _User Access Token_. This text explains how to do it.

### Register your Slack application

First you need to register your client application. 
  1. Goto https://api.slack.com/apps and login
  2. Click ***Create New App***
  3. Give the app a name and assign it to a workspace
  4. Click ***OAuth & Permissions*** (In the left menu)
  5. Under ***Scopes*** – ***User Token Scopes*** click ***Add an OAuth Scope*** and set the scopes: ***channels:history, channels:read, channels:write, chat:write, im:write, pins:write, search:read, users:read***. 
  6. At the top of the page, click ***Install App to Workspace***
  7. Click ***Allow***


### Create a provider for Slack OAuth in ***Decisions***
  1. Go to ***System > Integration > OAuth > Providers***   ,  click ***ADD OAUTH PROVIDER***
  2. Fill the form<br />
       &nbsp;&nbsp;   _Token Request URL_: https://slack.com/api/oauth.v2.access<br />
       &nbsp;&nbsp;   _Authorize URL_: https://slack.com/oauth/v2/authorize<br />
       &nbsp;&nbsp;     _Callback URL_: http://localhost/decisions/HandleTokenResponse<br />
  ![screenshot of sample](https://github.com/Decisions-Modules/Decisions.Slack/blob/master/CreatingProvider.png)


### Get AccessToken in ***Decisions***
  1. Go to ***System > Integration > OAuth > Tokens*** and click ***CREATE TOKEN***.
  2. Set ***Token Name*** value.
  3. Choose the provider you have created.
  4. Set ***Additional values*** to ***user_scope=channels:history,channels:read,channels:write,chat:write,im:write,pins:write,search:read,users:read***
  5. Click Request Token. A browser window will be open. Just follow the instructions in it.
![screenshot of sample](https://github.com/Decisions-Modules/Decisions.Slack/blob/master/CreatingToken.png)


### Read more:
https://documentation.decisions.com/docs/singlesignonwithgoogleidentityplatform  
https://api.slack.com/authentication/oauth-v2  
