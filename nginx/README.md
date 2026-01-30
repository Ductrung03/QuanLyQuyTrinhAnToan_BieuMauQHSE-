# Nginx Configuration Directory

## Directory Structure

```
nginx/
├── ssl/                    # SSL certificates (gitignored)
│   ├── certificate.crt
│   └── private.key
├── conf.d/                 # Additional nginx configurations
│   └── ssl.conf           # HTTPS configuration (optional)
└── README.md              # This file
```

## SSL Certificate Setup

### Option 1: Self-Signed Certificate (Development Only)

```bash
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout nginx/ssl/private.key \
  -out nginx/ssl/certificate.crt \
  -subj "/C=VN/ST=HCM/L=HCM/O=SSMS/CN=localhost"
```

### Option 2: Let's Encrypt (Production)

```bash
certbot certonly --standalone -d your-domain.com
cp /etc/letsencrypt/live/your-domain.com/fullchain.pem nginx/ssl/certificate.crt
cp /etc/letsencrypt/live/your-domain.com/privkey.pem nginx/ssl/private.key
```

### Option 3: Commercial Certificate

1. Purchase SSL certificate from provider
2. Download certificate files
3. Copy to `nginx/ssl/`:
   - `certificate.crt` - Certificate file
   - `private.key` - Private key file

## Enable HTTPS

1. Place SSL certificates in `nginx/ssl/`
2. Create `nginx/conf.d/ssl.conf` (see ssl.conf.example)
3. Restart nginx: `docker compose restart web`

## Security Notes

- Never commit SSL certificates to git
- `nginx/ssl/` is in .gitignore
- Use strong SSL ciphers (TLSv1.2+)
- Enable HSTS in production
