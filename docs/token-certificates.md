# Token signing certificates

## Overview

Gatekeeper uses a `X509Certificate2` to create and verify the tokens it issues.  These  are stored in the self-contained PXF format which includes both the certificate and the private key, and must be protected by an export password.

The application loads certificates from the location specified in the [runtime configuration](runtime-configuration.md).

## Creating a certificate

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

#### 4. Tell Gatekeeper where to find the certificate

If appropriate, move `is4cert.pfx` to another directory.

Follow the instructions in [runtime configuration](runtime-configuration.md) to set both the certificate directory, and certificate password.