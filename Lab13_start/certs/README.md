# Certificate self-signed pentru dev local

Pentru a rula `docker compose up` cu HTTPS local, generati un certificat self-signed:

```bash
openssl req -x509 -newkey rsa:4096 \
  -keyout server.key \
  -out server.crt \
  -days 365 \
  -nodes \
  -subj "/CN=localhost"
```

Plasati fisierele rezultate (`server.crt`, `server.key`) in acest folder.

Browser-ul va afisa un warning "Connection not private" - apasati "Advanced" -> "Proceed to localhost (unsafe)". Pentru productie, folositi un cert real (Let's Encrypt prin certbot).

**Important**: server.crt si server.key sunt in `.gitignore` - nu le committati niciodata.
