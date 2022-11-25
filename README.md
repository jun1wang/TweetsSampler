# TweetsSampler

## Introduction

TweetSampler is a tiny .NET Core application that

1) Consumes Twitter V2 Sample Stream API, which provides approximately 1% of the full tweet stream. 
2) Provides trending hashtags in real time.

## Components

### TweetSampler.Worker 

TweetSampler.Worker is a worker service that does nothing but hosting the processor which does all the hard work.

### TweetSampler.Core

TweetSampler.Core is a class library that calls Tweetinvi to access Twitter STREAM APIs. It receives Sample Tweet Stream and finds out the Top 10 most popular Hashtags. It also persist the results for the pourpose
of sharing to the public.

### TweetSampler.Model

TweetSampler.Model is also a class library that contains common models shared betwen TweetSampler.Core and TweetSampler.API.

### TweetSampler.API

TweetSampler.API is REST API that publishes the most popular Hashtags to the public.

## Usage

### 1. Run TweetSampler.Worker first

Assuming you've already set up your own Twetter project and application, you'll need to provide Twitter Bearer Token in appsettings.json before you can run the worker service.

  "TweetSamplerConfiguration": {
    "TwitterBearToken": "**REPLACE THIS**"
  }

Run the worker service for at least a few minutes. You'll see Data\TrendingHashTags.json periodically updated with the lastest top hashtags.

### 2. Run TweetSampler.API locally from Visual Studio (eventually on Cloud). 

### 3. Check results via Postman with URL below 

       http://localhost:5145/TrendingHashTags
