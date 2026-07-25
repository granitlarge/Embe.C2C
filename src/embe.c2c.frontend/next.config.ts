import type { NextConfig } from "next";
import { Routes } from "./src/shared/routes";

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
        pathname: "/images/**",
      }
    ],
  },
  allowedDevOrigins: ['192.168.8.7', 'frontend-embe.c2c.aspire.dev.localhost'],
  experimental: {
    serverActions: {
      bodySizeLimit: "30mb"
    }
  },
  async redirects() {
    return [
      {
        source: "/",
        destination: Routes.protected.search,
        permanent: false
      }
    ]
  }
};

export default nextConfig;
import('@opennextjs/cloudflare').then(m => m.initOpenNextCloudflareForDev());
