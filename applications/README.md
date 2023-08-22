# Project ASP.NET Core Docker Container Application

This sample demonstrates how to build container images for *** Project ASP.NET Core web apps***. 

## Try a pre-built sample container image

You can start by launching a sample from the [Microsoft container registry](https://mcr.microsoft.com/) and access it in a local web browser at `http://localhost:8080`.

```powershell
docker run --rm --interactive --tty --publish 8080:80 mcr.microsoft.com/dotnet/samples:aspnetapp
```

You can also call the endpoint that the app exposes:

```powershell
$ curl http://localhost:8080/Environment
{"runtimeVersion":".NET 7.0.9","osVersion":"Linux 5.10.16.3-microsoft-standard-WSL2 #1 SMP Fri Apr 2 22:23:49 UTC 2021","osArchitecture":"X64","user":"root","processorCount":8,"totalAvailableMemoryBytes":26672091136,"memoryLimit":9223372036854771712,"memoryUsage":28987392,"hostName":"541501cd8ab4"}
```

## Build a local Dockerfile based ASP.NET container image

You can build and run an image using the following instructions (if you've cloned this repo):

```powershell
docker build --pull --tag webapp .
```

You should see the following console output as the application starts:

```powershell
docker run --env ASPNETAPP_NAME=WebApp --env ASPNETAPP_FULLNAME="Application Name" --detach --publish 8080:80 webapp
b9f457ef9f46d9272aad1fd0fa9815c19074b2cbafaf442d02a0a3600a872c85
```

After the application starts, navigate to `http://localhost:8080` in your local web browser.

> Note: ASP.NET Core apps (in official images) listen to port 80 by default. The [`--publish` argument](https://docs.docker.com/engine/reference/commandline/run/#publish) in these examples maps host port `8080` to container port `80` (`host:container` mapping). The container will not be accessible without this mapping. 

You can see the app running via `docker ps`.

```powershell
> docker ps
CONTAINER ID   IMAGE     COMMAND                  CREATED         STATUS                           PORTS                  NAMES
42d58fd44526   webapp    "dotnet WebApp.dll -…"   3 seconds ago   Up 1 second (health: starting)   0.0.0.0:8080->80/tcp   happy_boy
```
## Supported Linux distros

The .NET Team publishes images for multiple distros. The default [Dockerfile](Dockerfile) uses a major.minor version tag, which references a multi-platform image that provides Debian and Windows Nano Server images (depending on the requesting client).

More extensive samples are provided at the [GitHub dotnet-docker repository](https://github.com/dotnet/dotnet-docker).

## Deploy a local Docker container image to OpenShift project

You can tag and push an image using the following instructions (if you've built the docker image from this repo):

```powershell
oc registry login
docker tag webapp image-registry.apps.silver.devops.gov.bc.ca/[Namespace-dev]/webappimage
docker push image-registry.apps.silver.devops.gov.bc.ca/[Namespace-dev]/webappimage:latest
```
Deployment Config YAML file
```
kind: DeploymentConfig
apiVersion: apps.openshift.io/v1
metadata:
  annotations:
    app.openshift.io/route-disabled: 'false'
    openshift.io/generated-by: OpenShiftWebConsole
  name: webapp
  namespace: [Namespace-dev]
  labels:
    app: webapp
    app.kubernetes.io/component: webapp
    app.kubernetes.io/instance: webapp
    app.kubernetes.io/name: webapp
    app.openshift.io/runtime-namespace: [Namespace-dev]
    app.openshift.io/runtime-version: latest
spec:
  strategy:
    type: Recreate
    resources: {}
    activeDeadlineSeconds: 21600
    recreateParams:
      timeoutSeconds: 600
  triggers:
    - type: ImageChange
      imageChangeParams:
        automatic: true
        containerNames:
          - webapp
        from:
          kind: ImageStreamTag
          name: 'webapp:latest'
          namespace: [Namespace-dev]
    - type: ConfigChange
  replicas: 1
  revisionHistoryLimit: 10
  test: false
  selector:
    app: webapp
    deploymentconfig: webapp
  template:
    metadata:
      creationTimestamp: null
      labels:
        app: webapp
        deploymentconfig: webapp
    spec:
      containers:
        - name: webapp
          image: >-
            image-registry.openshift-image-registry.svc:5000/[Namespace-dev]/webapp@sha256:0316b2530cfda6fa4902bd29d0d680f042370b6b100c71cb45fef4c6848ff4cd
          ports:
            - containerPort: 8080
              protocol: TCP
          env:
            - name: ASPNETCORE_URLS
              value: 'http://*:8080'
            - name: ASPNETAPP_NAME
              value: WebApp
            - name: ASPNETAPP_FULLNAME
              value: Application System Name
          resources:
            limits:
              cpu: 125m
              memory: 125Mi
            requests:
              cpu: 100m
              memory: 100Mi
          terminationMessagePath: /dev/termination-log
          terminationMessagePolicy: File
          imagePullPolicy: IfNotPresent
      restartPolicy: Always
      terminationGracePeriodSeconds: 30
      dnsPolicy: ClusterFirst
      securityContext: {}
      schedulerName: default-scheduler
      imagePullSecrets: []
  paused: false
```
Service YAML file
```
kind: Service
apiVersion: v1
metadata:
  name: webapp
  namespace: [Namespace-dev]
  labels:
    app: webapp
    app.kubernetes.io/component: webapp
    app.kubernetes.io/instance: webapp
    app.kubernetes.io/name: webapp
  annotations:
    openshift.io/generated-by: OpenShiftWebConsole
spec:
  ipFamilies:
    - IPv4
  ports:
    - name: 8080-tcp
      protocol: TCP
      port: 8080
      targetPort: 8080
  internalTrafficPolicy: Cluster
  clusterIPs:
    - 10.98.182.21
  type: ClusterIP
  ipFamilyPolicy: SingleStack
  sessionAffinity: None
  selector:
    app: webapp
    deploymentconfig: webapp
status:
  loadBalancer: {}
```
Route YAML file
```
kind: Route
apiVersion: route.openshift.io/v1
metadata:
  name: webapp
  namespace: [Namespace-dev]
  labels:
    app: webapp
    app.kubernetes.io/component: webapp
    app.kubernetes.io/instance: webapp
    app.kubernetes.io/name: webapp
  annotations:
    openshift.io/host.generated: 'true'
spec:
  host: webapp-[Namespace-dev].apps.silver.devops.gov.bc.ca
  to:
    kind: Service
    name: webapp
    weight: 100
  port:
    targetPort: 8080-tcp
  tls:
    termination: edge
    insecureEdgeTerminationPolicy: Redirect
  wildcardPolicy: None
status:
  ingress:
    - host: webapp-[Namespace-dev].apps.silver.devops.gov.bc.ca
      routerName: default
      conditions:
        - type: Admitted
          status: 'True'
      wildcardPolicy: None
      routerCanonicalHostname: router-default.apps.silver.devops.gov.bc.ca

```
