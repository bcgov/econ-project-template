# .NET Docker Sample

This sample demonstrates how to build container images for .NET console apps. See [.NET Docker Samples](../README.md) for more samples.

## Run the sample image

You can start by launching a sample from the Microsoft [Container Registry](https://mcr.microsoft.com/).

```console
docker run --rm mcr.microsoft.com/dotnet/samples:dotnetapp
```

This container image is built with Ubuntu, with [Dockerfile](Dockerfile).

## Build the image

You can build and run an image using the following instructions (if you've cloned this repo):

```console
docker build --pull -t dotnetapp .
docker run --rm dotnetapp
```
