﻿using System;
namespace UrlShortener.Cache
{
	public interface ICacheService
	{
		T GetData<T>(string key);

		bool SetData<T>(string key, T value, DateTimeOffset expirationTime);

		bool RemoveData<T>(string key);
	}
}

