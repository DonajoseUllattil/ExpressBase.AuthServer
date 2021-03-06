﻿using System.Collections.Generic;
using ExpressBase.Data;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Configuration;
using ServiceStack.Web;
using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Structures;
using ExpressBase.Common.ServiceStack.Auth;
using Newtonsoft.Json;
using System;
using System.Data.Common;
using System.Threading.Tasks;
using ExpressBase.Common.Extensions;

namespace ExpressBase.ServiceStack
{
    public class MyTwitterAuthProvider : TwitterAuthProvider
    {

        public MyTwitterAuthProvider(IAppSettings settings) : base(settings) { }

        public override object Authenticate(IServiceBase authService, IAuthSession session, Authenticate request)
        {
            var objret = base.Authenticate(authService, session, request);
           
            // no need if email present change to email

                if (!string.IsNullOrEmpty(session.ProviderOAuthAccess[0].DisplayName))
            {
                EbConnectionFactory InfraConnectionFactory = authService.ResolveService<IEbConnectionFactory>() as EbConnectionFactory;

                if (!string.IsNullOrEmpty(session.ProviderOAuthAccess[0].Email))
                {
                    string b = string.Empty;
                    try
                    {
                        bool unique = false;
                        string sql1 = "SELECT id, pwd FROM eb_tenants WHERE email ~* @email";
                        DbParameter[] parameters2 = { InfraConnectionFactory.DataDB.GetNewParameter("email", EbDbTypes.String, session.ProviderOAuthAccess[0].Email) };
                        EbDataTable dt = InfraConnectionFactory.DataDB.DoQuery(sql1, parameters2);
                        if (dt.Rows.Count > 0)
                        {
                            unique = false;
                        }
                        else
                            unique = true;
                        //string sql11 = "SELECT id, pwd FROM eb_tenants WHERE email ~* @email";
                        //DbParameter[] parameters22 = { InfraConnectionFactory.DataDB.GetNewParameter("email", EbDbTypes.String, session.ProviderOAuthAccess[0].Email) };
                        //int dtq = InfraConnectionFactory.DataDB.DoNonQuery(sql11, parameters22);
                        if (unique == true)
                        {
                            DbParameter[] parameter1 = {
                            InfraConnectionFactory.DataDB.GetNewParameter("email", EbDbTypes.String,  session.ProviderOAuthAccess[0].Email),
                            InfraConnectionFactory.DataDB.GetNewParameter("name", EbDbTypes.String,  session.ProviderOAuthAccess[0].UserName),
                             InfraConnectionFactory.DataDB.GetNewParameter("twitterid", EbDbTypes.String,  (session.ProviderOAuthAccess[0].UserId).ToString()),
                             InfraConnectionFactory.DataDB.GetNewParameter("password", EbDbTypes.String,  (session.ProviderOAuthAccess[0].UserId.ToString()+session.ProviderOAuthAccess[0].Email.ToString()).ToMD5Hash()),

                             };

                            EbDataTable dtbl = InfraConnectionFactory.DataDB.DoQuery(@"INSERT INTO eb_tenants (email,fullname,twitter_id,pwd) 
                             VALUES 
                             (:email,:name,:twitterid,:password) RETURNING id;", parameter1);


                        }
                        SocialSignup sco_signup = new SocialSignup
                        {
                            AuthProvider = session.ProviderOAuthAccess[0].Provider,
                            Country = session.ProviderOAuthAccess[0].Country,
                            Email = session.ProviderOAuthAccess[0].Email,
                            Social_id = (session.ProviderOAuthAccess[0].UserId).ToString(),
                            Fullname = session.ProviderOAuthAccess[0].UserName,
                            //IsVerified = session.IsAuthenticated,
                            Pauto = (session.ProviderOAuthAccess[0].UserId.ToString() + session.ProviderOAuthAccess[0].Email.ToString()).ToMD5Hash(),
                            UniqueEmail = unique,
                        };
                        b = JsonConvert.SerializeObject(sco_signup);
                        return authService.Redirect(SuccessRedirectUrlFilter(this, string.Format("http://localhost:41500/social_oauth?scosignup={0}", b)));

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception: " + e.Message + e.StackTrace);
                    }
                }
                //using (var con = _InfraDb.DataDB.GetNewConnection())
                //{
                //    con.Open();
                //    var cmd = _InfraDb.DataDB.GetNewCommand(con, "INSERT INTO eb_users (email,firstname,socialid,prolink) VALUES(@email, @firstname,@socialid,@prolink) ON CONFLICT(socialid) DO UPDATE SET loginattempts = eb_users.loginattempts + EXCLUDED.loginattempts RETURNING eb_users.loginattempts");
                //    cmd.Parameters.Add(_InfraDb.DataDB.GetNewParameter("email", EbDbTypes.String, session.ProviderOAuthAccess[0].Email));
                //    cmd.Parameters.Add(_InfraDb.DataDB.GetNewParameter("firstname", EbDbTypes.String, session.ProviderOAuthAccess[0].DisplayName));
                //    cmd.Parameters.Add(_InfraDb.DataDB.GetNewParameter("socialid", EbDbTypes.String, session.ProviderOAuthAccess[0].UserName));
                //    cmd.Parameters.Add(_InfraDb.DataDB.GetNewParameter("prolink", EbDbTypes.String, session.ProviderOAuthAccess[0].Items["profileUrl"]));
                //    cmd.ExecuteNonQuery();
                //}

                //(session as CustomUserSession).Company = CoreConstants.EXPRESSBASE;
                //(session as CustomUserSession).WhichConsole = "tc";
                //return authService.Redirect(SuccessRedirectUrlFilter(this, "http://localhost:5000/Ext/AfterSignInSocial?email=" + session.Email + "&socialId=" + session.UserName + "&provider=" + session.AuthProvider + "&providerToken=" + session.ProviderOAuthAccess[0].AccessTokenSecret));
            }

            return objret;
        }

        public override string CreateOrMergeAuthSession(IAuthSession session, IAuthTokens tokens)
        {
            return base.CreateOrMergeAuthSession(session, tokens);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool IsAccountLocked(IAuthRepository authRepo, IUserAuth userAuth, IAuthTokens tokens = null)
        {
            return base.IsAccountLocked(authRepo, userAuth, tokens);
        }

        public override bool IsAuthorized(IAuthSession session, IAuthTokens tokens, Authenticate request = null)
        {
            return base.IsAuthorized(session, tokens, request);
        }

        public override void LoadUserOAuthProvider(IAuthSession authSession, IAuthTokens tokens)
        {
            base.LoadUserOAuthProvider(authSession, tokens);
        }

        public override object Logout(IServiceBase service, Authenticate request)
        {
            return base.Logout(service, request);
        }





        //************is this for authanticate********


        public override IHttpResult OnAuthenticated(IServiceBase authService, IAuthSession session, IAuthTokens tokens, Dictionary<string, string> authInfo)
        {
            return base.OnAuthenticated(authService, session, tokens, authInfo);
        }

        public override void OnFailedAuthentication(IAuthSession session, IRequest httpReq, IResponse httpRes)
        {
            base.OnFailedAuthentication(session, httpReq, httpRes);
        }

        public override Task OnFailedAuthenticationAsync(IAuthSession session, IRequest httpReq, IResponse httpRes)
        {
            return base.OnFailedAuthenticationAsync(session, httpReq, httpRes);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        protected override object ConvertToClientError(object failedResult, bool isHtml)
        {
            return base.ConvertToClientError(failedResult, isHtml);
        }

        protected override bool EmailAlreadyExists(IAuthRepository authRepo, IUserAuth userAuth, IAuthTokens tokens = null)
        {
            return base.EmailAlreadyExists(authRepo, userAuth, tokens);
        }

        protected override string GetAuthRedirectUrl(IServiceBase authService, IAuthSession session)
        {
            return base.GetAuthRedirectUrl(authService, session);
        }

        protected override IAuthRepository GetAuthRepository(IRequest req)
        {
            return base.GetAuthRepository(req);
        }

        protected override string GetReferrerUrl(IServiceBase authService, IAuthSession session, Authenticate request = null)
        {
            return base.GetReferrerUrl(authService, session, request);
        }

        protected override void LoadUserAuthInfo(AuthUserSession userSession, IAuthTokens tokens, Dictionary<string, string> authInfo)
        {
            base.LoadUserAuthInfo(userSession, tokens, authInfo);
        }

        protected override bool UserNameAlreadyExists(IAuthRepository authRepo, IUserAuth userAuth, IAuthTokens tokens = null)
        {
            return base.UserNameAlreadyExists(authRepo, userAuth, tokens);
        }

        protected override IHttpResult ValidateAccount(IServiceBase authService, IAuthRepository authRepo, IAuthSession session, IAuthTokens tokens)
        {
            return base.ValidateAccount(authService, authRepo, session, tokens);
        }
    }
}