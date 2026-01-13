/**
 * URL Configuration for OnboardingQuestionnaire
 * Uses centralized config pattern - matches shahin-ai-website/lib/config.ts
 */

// Standardized on portal.shahin-ai.com (matches ASP.NET backend)
export const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 
  (process.env.NODE_ENV === 'production' 
    ? 'https://portal.shahin-ai.com' 
    : 'http://localhost:8080');
