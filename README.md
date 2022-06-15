Ark Metrics Exporter
====================

The purpose of this application is to provide an easy to use exporter for ARK: Survival Evolved to publish metrics that are Prometheus formatted for consumption for use in that stack including use in Grafana dashboards or Alert-Manager for notifications.

Software Build
--------------

This software is designed using .Net Core and thus should work in all operating systems that can compile and run .Net Core modules.  At this time of this documentation, the only testing done has been on Windows, however Linux and Mac should both be fully supported.

Application Configuration
-------------------------

This application expects to find a *config.yaml* file in the working directory of the application on startup.  That configuration file allows for configuration of multiple IP addresses and query ports for handling exporting of multiple servers.  An example configuration would look similar to the following:

```yaml
prometheus-port: 7777
servers:
  - ip-address: "192.168.1.1"
    port: 8815
  - ip-address: "192.168.1.1"
    port: 8820
  - ip-address: "192.168.1.10"
    port: 5290
```

Docker Image
------------

This application can be run as a docker container with the following configuration:
```bash
docker run -d -p 7777:7777 \
    -v /path/to/config.yaml:/app/config.yaml \
    gamecodeplus/ark-metrics-exporter
```

Notes
-----
Currently the application will not export the first set of results for approximately 30s after the application has started.  The metrics export page should exist at http://{ip address}:{prometheus-port}/metrics however the metrics data will not exist immediately as the servers are queried for the first time.
