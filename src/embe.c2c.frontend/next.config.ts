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
        pathname: "/files/**", 
      }
    ],
  },
  experimental: {
    serverActions: {
      bodySizeLimit: "10mb"
    }
  },
  allowedDevOrigins: ['192.168.8.7'],
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