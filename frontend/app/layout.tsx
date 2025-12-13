import type { Metadata } from "next";
import Script from "next/script";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import ClientProviders from "@/components/providers/ClientProviders";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "SSMS - Hệ thống Quản lý An toàn QHSE",
  description: "Hệ thống quản lý quy trình an toàn và biểu mẫu QHSE",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="vi">
      <head>
        {/* Fallback for Tailwind CSS due to path issues with v4 build */}
      </head>
      <body
        className={`${geistSans.variable} ${geistMono.variable} antialiased`}
      >
        <Script src="https://cdn.tailwindcss.com" strategy="beforeInteractive" />
        <ClientProviders>
          {children}
        </ClientProviders>
      </body>
    </html>
  );
}
