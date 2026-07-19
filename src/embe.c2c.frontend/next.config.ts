import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  images: {
    remotePatterns: [
      {
        protocol: "http",
        hostname: "127.0.0.1",
        port: "10000",
        pathname: "/**",
      },
      {
        protocol: "https",
        hostname: "relationshipfindersa.blob.core.windows.net",
        pathname: "/images/pending/**",
      },
      {
        protocol: "https",
        hostname: "relationshipfindersa.blob.core.windows.net",
        pathname: "/images/accepted/**",
      }
    ],
  },
  allowedDevOrigins: ['192.168.8.7', 'frontend-embe.c2c.aspire.dev.localhost'],
  async redirects() {
    return [
      {
        source: "/",
        destination: "/protected/search",
        permanent: false
      }
    ]
  }
};

export default nextConfig;
import('@opennextjs/cloudflare').then(m => m.initOpenNextCloudflareForDev());
