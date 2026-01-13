"use client"

import { useEffect, useRef, useState } from "react"
import { motion } from "framer-motion"
import { BarChart3, RefreshCw, Maximize2, ExternalLink } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { useTranslations } from "next-intl"

interface SupersetEmbedProps {
  dashboardId: string
  title?: string
  height?: string
  showToolbar?: boolean
  filters?: Record<string, string>
}

export function SupersetEmbed({
  dashboardId,
  title,
  height = "600px",
  showToolbar = true,
  filters = {},
}: SupersetEmbedProps) {
  // All hooks must be called before any conditional returns
  const iframeRef = useRef<HTMLIFrameElement>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [isFullscreen, setIsFullscreen] = useState(false)
  const t = useTranslations("dashboard")

  useEffect(() => {
    const handler = () => {
      setIsFullscreen(!!document.fullscreenElement)
    }
    document.addEventListener("fullscreenchange", handler)
    return () => {
      document.removeEventListener("fullscreenchange", handler)
    }
  }, [])

  // Compute derived values after hooks
  const displayTitle = title || t("analyticsBoard")
  const supersetUrl = process.env.NEXT_PUBLIC_SUPERSET_URL
  
  // Analytics URLs are optional - show message if not configured
  if (!supersetUrl && process.env.NODE_ENV === 'production') {
    return (
      <Card className="overflow-hidden">
        <CardContent className="p-8 text-center text-muted-foreground">
          <p>Analytics dashboard is not configured. Please set NEXT_PUBLIC_SUPERSET_URL environment variable.</p>
        </CardContent>
      </Card>
    )
  }
  
  // For development, allow localhost fallback with warning
  const effectiveUrl = supersetUrl || (process.env.NODE_ENV === 'development' ? 'http://localhost:8088' : undefined)
  
  if (!effectiveUrl) {
    return null
  }

  // Build embed URL with filters
  const buildEmbedUrl = () => {
    const params = new URLSearchParams({
      standalone: "true",
      show_filters: "false",
      ...filters,
    })
    return `${effectiveUrl}/superset/dashboard/${dashboardId}/?${params.toString()}`
  }

  const handleRefresh = () => {
    if (iframeRef.current) {
      setIsLoading(true)
      iframeRef.current.src = buildEmbedUrl()
    }
  }

  const handleFullscreen = () => {
    if (iframeRef.current) {
      if (iframeRef.current.requestFullscreen) {
        iframeRef.current.requestFullscreen()
        setIsFullscreen(true)
      }
    }
  }

  const handleOpenExternal = () => {
    window.open(`${effectiveUrl}/superset/dashboard/${dashboardId}/`, "_blank")
  }

  return (
    <Card className="overflow-hidden">
      {showToolbar && (
        <CardHeader className="flex flex-row items-center justify-between py-3 px-4 border-b">
          <CardTitle className="text-lg font-semibold flex items-center gap-2">
            <BarChart3 className="w-5 h-5 text-emerald-500" />
            {displayTitle}
          </CardTitle>
          <div className="flex items-center gap-2">
            <Button
              variant="ghost"
              size="sm"
              onClick={handleRefresh}
              className="h-8 w-8 p-0"
              title={t("refresh")}
            >
              <RefreshCw className={`w-4 h-4 ${isLoading ? "animate-spin" : ""}`} />
            </Button>
            <Button
              variant="ghost"
              size="sm"
              onClick={handleFullscreen}
              className="h-8 w-8 p-0"
              title={t("fullscreen")}
            >
              <Maximize2 className="w-4 h-4" />
            </Button>
            <Button
              variant="ghost"
              size="sm"
              onClick={handleOpenExternal}
              className="h-8 w-8 p-0"
              title={t("openNewWindow")}
            >
              <ExternalLink className="w-4 h-4" />
            </Button>
          </div>
        </CardHeader>
      )}
      <CardContent className="p-0 relative">
        {isLoading && (
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            className="absolute inset-0 flex items-center justify-center bg-gray-50 dark:bg-gray-800 z-10"
          >
            <div className="flex flex-col items-center gap-3">
              <div className="w-10 h-10 border-4 border-emerald-500 border-t-transparent rounded-full animate-spin" />
              <span className="text-sm text-gray-500">{t("loadingAnalytics")}</span>
            </div>
          </motion.div>
        )}
        <iframe
          ref={iframeRef}
          src={buildEmbedUrl()}
          width="100%"
          height={height}
          frameBorder="0"
          onLoad={() => setIsLoading(false)}
          className="w-full"
          title={displayTitle}
          allow="fullscreen"
        />
      </CardContent>
    </Card>
  )
}

// Grafana Embed Component
interface GrafanaEmbedProps {
  dashboardUid: string
  panelId?: number
  title?: string
  height?: string
  from?: string
  to?: string
  refresh?: string
}

export function GrafanaEmbed({
  dashboardUid,
  panelId,
  title,
  height = "400px",
  from = "now-24h",
  to = "now",
  refresh = "30s",
}: GrafanaEmbedProps) {
  const [isLoading, setIsLoading] = useState(true)
  const t = useTranslations("dashboard")
  const displayTitle = title || t("systemMonitoring")
  const grafanaUrl = process.env.NEXT_PUBLIC_GRAFANA_URL
  
  // Analytics URLs are optional - show message if not configured in production
  if (!grafanaUrl && process.env.NODE_ENV === 'production') {
    return (
      <Card className="overflow-hidden">
        <CardContent className="p-8 text-center text-muted-foreground">
          <p>Analytics dashboard is not configured. Please set NEXT_PUBLIC_GRAFANA_URL environment variable.</p>
        </CardContent>
      </Card>
    )
  }
  
  // For development, allow localhost fallback
  const effectiveGrafanaUrl = grafanaUrl || (process.env.NODE_ENV === 'development' ? 'http://localhost:3030' : undefined)
  
  if (!effectiveGrafanaUrl) {
    return null
  }

  const buildEmbedUrl = () => {
    const params = new URLSearchParams({
      orgId: "1",
      from,
      to,
      theme: "light",
      refresh,
    })

    if (panelId) {
      return `${effectiveGrafanaUrl}/d-solo/${dashboardUid}?${params.toString()}&panelId=${panelId}`
    }
    return `${effectiveGrafanaUrl}/d/${dashboardUid}?${params.toString()}&kiosk`
  }

  return (
    <Card className="overflow-hidden">
      <CardHeader className="py-3 px-4 border-b">
        <CardTitle className="text-lg font-semibold flex items-center gap-2">
          <BarChart3 className="w-5 h-5 text-blue-500" />
          {displayTitle}
        </CardTitle>
      </CardHeader>
      <CardContent className="p-0 relative">
        {isLoading && (
          <div className="absolute inset-0 flex items-center justify-center bg-gray-50 dark:bg-gray-800 z-10">
            <div className="w-8 h-8 border-4 border-blue-500 border-t-transparent rounded-full animate-spin" />
          </div>
        )}
        <iframe
          src={buildEmbedUrl()}
          width="100%"
          height={height}
          frameBorder="0"
          onLoad={() => setIsLoading(false)}
          className="w-full"
          title={displayTitle}
        />
      </CardContent>
    </Card>
  )
}

// Metabase Embed Component
interface MetabaseEmbedProps {
  questionId?: number
  dashboardId?: number
  title?: string
  height?: string
}

export function MetabaseEmbed({
  questionId,
  dashboardId,
  title,
  height = "500px",
}: MetabaseEmbedProps) {
  const [isLoading, setIsLoading] = useState(true)
  const t = useTranslations("dashboard")
  const displayTitle = title || t("analyticalReport")
  const metabaseUrl = process.env.NEXT_PUBLIC_METABASE_URL
  
  // Analytics URLs are optional - show message if not configured in production
  if (!metabaseUrl && process.env.NODE_ENV === 'production') {
    return (
      <Card className="overflow-hidden">
        <CardContent className="p-8 text-center text-muted-foreground">
          <p>Analytics dashboard is not configured. Please set NEXT_PUBLIC_METABASE_URL environment variable.</p>
        </CardContent>
      </Card>
    )
  }
  
  // For development, allow localhost fallback
  const effectiveMetabaseUrl = metabaseUrl || (process.env.NODE_ENV === 'development' ? 'http://localhost:3033' : undefined)
  
  if (!effectiveMetabaseUrl) {
    return null
  }

  const buildEmbedUrl = () => {
    if (dashboardId) {
      return `${effectiveMetabaseUrl}/public/dashboard/${dashboardId}`
    }
    if (questionId) {
      return `${effectiveMetabaseUrl}/public/question/${questionId}`
    }
    return effectiveMetabaseUrl
  }

  return (
    <Card className="overflow-hidden">
      <CardHeader className="py-3 px-4 border-b">
        <CardTitle className="text-lg font-semibold">{displayTitle}</CardTitle>
      </CardHeader>
      <CardContent className="p-0 relative">
        {isLoading && (
          <div className="absolute inset-0 flex items-center justify-center bg-gray-50 dark:bg-gray-800 z-10">
            <div className="w-8 h-8 border-4 border-purple-500 border-t-transparent rounded-full animate-spin" />
          </div>
        )}
        <iframe
          src={buildEmbedUrl()}
          width="100%"
          height={height}
          frameBorder="0"
          onLoad={() => setIsLoading(false)}
          className="w-full"
          title={displayTitle}
        />
      </CardContent>
    </Card>
  )
}
