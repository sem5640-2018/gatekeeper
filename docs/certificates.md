# Application certificates

## Overview

Gatekeeper uses two `X509Certificate2` certificates:
 * `is4cert.pfx` is used by IdentityServer4 to create and verify the tokens it issues.
 * `dpkcert.pfx` is used to protect the Data Protection Keys which are self-managed by .NET Core.

These self-contained PXF files contain both a certificate and a private key, and must be protected by an export password.

The application loads certificates from the location specified in the [runtime configuration](runtime-configuration.md).

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

There are two ways in which the application can load certificates.  Using the certificate file, or from the local machine certificate store.  Loading from file is much easier, and is the preferred method.

### Loading Certificates from File

**Windows Users**: There is a bug in .NET Core 2.1 preventing correct loading of certificates from files.  You must use the [Keystore](#Loading-Certificates-from-Keystore) method.

Move your `is4cert.pfx` and `dpkcert.pfx` files to a directory the application has permission to read.

Follow the instructions in [runtime configuration](runtime-configuration.md) to use the `CertStorageType` of `File`, ensuring that you set the optional keys required for this storage type.

### Loading Certificates from Keystore

**Linux Users**: This method is only implemented for Windows users due to the bug mentioned in the [File](#Loading-Certificates-from-File) method.  Use the File method instead.

Follow the instructions at [this stackoverflow solution](https://stackoverflow.com/a/21148852) to install both `is4cert.pfx` and `dpkcert.pfx` in your local machine keystore.

Follow the instructions in [runtime configuration](runtime-configuration.md) to use the `CertStorageType` of `Store`, ensuring that you set the optional keys required for this storage type.