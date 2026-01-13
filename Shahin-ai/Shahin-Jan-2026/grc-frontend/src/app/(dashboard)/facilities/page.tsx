"use client"

import { useState } from "react"
import { motion } from "framer-motion"
import {
  Building,
  MapPin,
  Users,
  Shield,
  Plus,
  Search,
  Filter,
  Calendar,
  AlertTriangle,
  CheckCircle,
  TrendingUp,
} from "lucide-react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"

// Mock data - replace with API calls
const facilities = [
  {
    id: "1",
    name: "مكتب الرياض الرئيسي",
    code: "RYD-HQ-001",
    type: "Office",
    status: "Active",
    city: "الرياض",
    country: "المملكة العربية السعودية",
    manager: "أحمد المحمد",
    capacity: 200,
    currentOccupancy: 185,
    securityLevel: "High",
    nextInspection: "2026-02-15",
    isInspectionDue: false,
  },
  {
    id: "2",
    name: "مركز بيانات جدة",
    code: "JED-DC-001",
    type: "DataCenter",
    status: "Active",
    city: "جدة",
    country: "المملكة العربية السعودية",
    manager: "فاطمة السالم",
    capacity: 50,
    currentOccupancy: 48,
    securityLevel: "Critical",
    nextInspection: "2026-01-20",
    isInspectionDue: true,
  },
  {
    id: "3",
    name: "فرع دبي",
    code: "DXB-BR-001",
    type: "Branch",
    status: "Active",
    city: "دبي",
    country: "الإمارات",
    manager: "محمد الشمري",
    capacity: 100,
    currentOccupancy: 75,
    securityLevel: "Medium",
    nextInspection: "2026-03-10",
    isInspectionDue: false,
  },
]

const stats = [
  {
    title: "إجمالي المرافق",
    value: "12",
    change: "+2",
    trend: "up",
    icon: Building,
    color: "blue",
  },
  {
    title: "مرافق نشطة",
    value: "10",
    change: "+1",
    trend: "up",
    icon: CheckCircle,
    color: "emerald",
  },
  {
    title: "تحتاج تفتيش",
    value: "3",
    change: "-1",
    trend: "down",
    icon: AlertTriangle,
    color: "amber",
  },
  {
    title: "إجمالي السعة",
    value: "850",
    change: "+50",
    trend: "up",
    icon: Users,
    color: "purple",
  },
]

const facilityTypes = [
  { value: "all", label: "جميع الأنواع" },
  { value: "Office", label: "مكتب" },
  { value: "DataCenter", label: "مركز بيانات" },
  { value: "Warehouse", label: "مستودع" },
  { value: "Branch", label: "فرع" },
  { value: "RemoteSite", label: "موقع بعيد" },
]

export default function FacilitiesPage() {
  const [searchQuery, setSearchQuery] = useState("")
  const [selectedType, setSelectedType] = useState("all")
  const [selectedStatus, setSelectedStatus] = useState("all")

  const filteredFacilities = facilities.filter((facility) => {
    const matchesSearch =
      facility.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
      facility.code.toLowerCase().includes(searchQuery.toLowerCase())
    const matchesType = selectedType === "all" || facility.type === selectedType
    const matchesStatus = selectedStatus === "all" || facility.status === selectedStatus
    return matchesSearch && matchesType && matchesStatus
  })

  const getSecurityLevelColor = (level: string) => {
    switch (level) {
      case "Critical":
        return "critical"
      case "High":
        return "warning"
      case "Medium":
        return "info"
      default:
        return "secondary"
    }
  }

  const getSecurityLevelText = (level: string) => {
    switch (level) {
      case "Critical":
        return "حرج"
      case "High":
        return "عالي"
      case "Medium":
        return "متوسط"
      case "Low":
        return "منخفض"
      default:
        return level
    }
  }

  const getFacilityTypeText = (type: string) => {
    switch (type) {
      case "Office":
        return "مكتب"
      case "DataCenter":
        return "مركز بيانات"
      case "Warehouse":
        return "مستودع"
      case "Branch":
        return "فرع"
      case "RemoteSite":
        return "موقع بعيد"
      default:
        return type
    }
  }

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">إدارة المرافق</h1>
          <p className="text-gray-600 dark:text-gray-400 mt-1">
            إدارة ومراقبة جميع مرافق المؤسسة
          </p>
        </div>
        <Button className="bg-red-600 hover:bg-red-700 text-white">
          <Plus className="w-4 h-4 ml-2" />
          إضافة مرفق جديد
        </Button>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {stats.map((stat) => (
          <motion.div
            key={stat.title}
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
          >
            <Card hover>
              <CardContent className="p-6">
                <div className="flex items-start justify-between">
                  <div>
                    <p className="text-sm font-medium text-gray-600 dark:text-gray-400 mb-1">
                      {stat.title}
                    </p>
                    <p className="text-3xl font-bold text-gray-900 dark:text-white">
                      {stat.value}
                    </p>
                    <div className="flex items-center gap-1 mt-2 text-sm text-emerald-600">
                      <TrendingUp className="w-4 h-4" />
                      <span>{stat.change} من الشهر الماضي</span>
                    </div>
                  </div>
                  <div
                    className={`w-12 h-12 rounded-xl flex items-center justify-center bg-${stat.color}-100 dark:bg-${stat.color}-900/20`}
                  >
                    <stat.icon className={`w-6 h-6 text-${stat.color}-600`} />
                  </div>
                </div>
              </CardContent>
            </Card>
          </motion.div>
        ))}
      </div>

      {/* Filters */}
      <Card>
        <CardContent className="p-6">
          <div className="flex flex-col sm:flex-row gap-4">
            <div className="flex-1">
              <div className="relative">
                <Search className="absolute right-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
                <Input
                  type="search"
                  placeholder="بحث عن مرفق..."
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  className="pr-10"
                />
              </div>
            </div>
            <select
              value={selectedType}
              onChange={(e) => setSelectedType(e.target.value)}
              className="px-4 py-2 rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800"
            >
              {facilityTypes.map((type) => (
                <option key={type.value} value={type.value}>
                  {type.label}
                </option>
              ))}
            </select>
            <select
              value={selectedStatus}
              onChange={(e) => setSelectedStatus(e.target.value)}
              className="px-4 py-2 rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800"
            >
              <option value="all">جميع الحالات</option>
              <option value="Active">نشط</option>
              <option value="Inactive">غير نشط</option>
            </select>
          </div>
        </CardContent>
      </Card>

      {/* Facilities Grid */}
      <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
        {filteredFacilities.map((facility) => (
          <motion.div
            key={facility.id}
            initial={{ opacity: 0, scale: 0.95 }}
            animate={{ opacity: 1, scale: 1 }}
          >
            <Card hover className="h-full">
              <CardHeader className="flex flex-row items-start justify-between space-y-0">
                <div className="flex-1">
                  <CardTitle className="text-lg font-bold">{facility.name}</CardTitle>
                  <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">
                    {facility.code}
                  </p>
                </div>
                {facility.isInspectionDue && (
                  <Badge variant="critical">تفتيش متأخر</Badge>
                )}
              </CardHeader>
              <CardContent className="space-y-4">
                {/* Type and Status */}
                <div className="flex items-center gap-2">
                  <Badge variant="secondary">{getFacilityTypeText(facility.type)}</Badge>
                  <Badge variant={facility.status === "Active" ? "success" : "warning"}>
                    {facility.status === "Active" ? "نشط" : "غير نشط"}
                  </Badge>
                </div>

                {/* Location */}
                <div className="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400">
                  <MapPin className="w-4 h-4" />
                  <span>
                    {facility.city}, {facility.country}
                  </span>
                </div>

                {/* Manager */}
                <div className="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400">
                  <Users className="w-4 h-4" />
                  <span>المدير: {facility.manager}</span>
                </div>

                {/* Capacity */}
                <div className="space-y-2">
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-600 dark:text-gray-400">الإشغال</span>
                    <span className="font-medium">
                      {facility.currentOccupancy} / {facility.capacity}
                    </span>
                  </div>
                  <div className="h-2 bg-gray-200 dark:bg-gray-700 rounded-full overflow-hidden">
                    <div
                      className="h-full bg-blue-600 rounded-full"
                      style={{
                        width: `${(facility.currentOccupancy / facility.capacity) * 100}%`,
                      }}
                    />
                  </div>
                </div>

                {/* Security Level */}
                <div className="flex items-center gap-2 text-sm">
                  <Shield className="w-4 h-4 text-gray-400" />
                  <span className="text-gray-600 dark:text-gray-400">الأمان:</span>
                  <Badge variant={getSecurityLevelColor(facility.securityLevel)}>
                    {getSecurityLevelText(facility.securityLevel)}
                  </Badge>
                </div>

                {/* Next Inspection */}
                <div className="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400">
                  <Calendar className="w-4 h-4" />
                  <span>التفتيش القادم: {facility.nextInspection}</span>
                </div>

                {/* Actions */}
                <div className="pt-4 border-t border-gray-200 dark:border-gray-700">
                  <Button variant="outline" className="w-full">
                    عرض التفاصيل
                  </Button>
                </div>
              </CardContent>
            </Card>
          </motion.div>
        ))}
      </div>

      {filteredFacilities.length === 0 && (
        <Card>
          <CardContent className="p-12 text-center">
            <Building className="w-16 h-16 text-gray-300 dark:text-gray-600 mx-auto mb-4" />
            <h3 className="text-lg font-medium text-gray-900 dark:text-white mb-2">
              لم يتم العثور على مرافق
            </h3>
            <p className="text-gray-600 dark:text-gray-400 mb-6">
              جرب تغيير معايير البحث أو الفلتر
            </p>
            <Button className="bg-red-600 hover:bg-red-700 text-white">
              <Plus className="w-4 h-4 ml-2" />
              إضافة مرفق جديد
            </Button>
          </CardContent>
        </Card>
      )}
    </div>
  )
}
