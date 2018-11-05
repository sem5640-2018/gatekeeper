# Application certificates

## Overview

In Development, certificates are automatically generated and handled for you.  All you need to do is set the `Development` certificate storage type in the [runtime configuration](runtime-configuration.md).

For other environments (staging and production) this is not appropriate, so you must create the certificates and ensure they are persisted correctly.

Gatekeeper uses two `X509Certificate2` certificates:
 * `is4cert.pfx` is used by IdentityServer4 to create and verify the tokens it issues.
 * `dpkcert.pfx` is used to protect the Data Protection Keys which are self-managed by .NET Core.

These self-contained PXF files contain both a certificate and a private key, and must be protected by an export password.

## Creating a certificate

**These instructions show the generation of `is4cert.pfx`.  Generating `dpkcert.pfx` uses the exact same process, you just need to change any instance of `is4cert` to `dpkcert`.**

#### 1. Install OpenSSL
Linux/MacOS users probably already have this installed.  The easiest way for Windows users is to use [cmder](http://cmder.net/) which includes OpenSSL.

#### 2. Generate the certificate & private key

From your terminal, run the following command:

`openssl req -x509 -newkey rsa:4096 -sha256 -nodes -keyout is4cert.key -out is4cert.crt -subj "/CN=aberfitness.dcs.aber.ac.uk" -days 3650`

This will save two files, `is4cert.key` and `is4cert.crt` in your current working directory.

#### 3. Convert the certificate & private key into a PFX

From your terminal, run the following command:

`openssl pkcs12 -export -out is4cert.pfx -inkey is4cert.key -in is4cert.crt -certfile is4cert.crt`

At this stage, you will be asked to set the export passsword.  This will be required by the application, so make sure you securely store it somewhere for later use.

You should now have `is4cert.pfx` in your current working directory.

## Using Certificates

Certificates must be stored in a location accessible to the application.  For staging/production deployments, the certificate should typically remain the same throughout deployments - this means that they should be persisted in an appropriate place such as docker volumes.

Follow the instructions in [runtime configuration](runtime-configuration.md) to set the `File` certificate storage type, ensuring that you also set the optional keys required for this storage type.