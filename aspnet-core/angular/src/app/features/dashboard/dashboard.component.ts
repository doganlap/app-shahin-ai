import { Component, OnInit } from '@angular/core';
import { DashboardService, DashboardOverviewDto, MyControlDto, FrameworkProgressDto } from './dashboard.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  overview: DashboardOverviewDto | null = null;
  myControls: MyControlDto[] = [];
  frameworkProgress: FrameworkProgressDto[] = [];
  loading = true;
  isRTL = true; // Support Arabic RTL

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.loading = true;

    // Load overview
    this.dashboardService.getOverview().subscribe({
      next: (data) => {
        this.overview = data;
      },
      error: (error) => {
        console.error('Error loading overview:', error);
        this.overview = this.getMockOverview();
      }
    });

    // Load my controls
    this.dashboardService.getMyControls().subscribe({
      next: (data) => {
        this.myControls = data;
      },
      error: (error) => {
        console.error('Error loading controls:', error);
        this.myControls = this.getMockControls();
      }
    });

    // Load framework progress
    this.dashboardService.getFrameworkProgress().subscribe({
      next: (data) => {
        this.frameworkProgress = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading framework progress:', error);
        this.frameworkProgress = this.getMockFrameworkProgress();
        this.loading = false;
      }
    });
  }

  getComplianceColor(level: string): string {
    const colors: { [key: string]: string } = {
      'High': 'success',
      'Medium': 'warning',
      'Low': 'danger',
      'عالي': 'success',
      'متوسط': 'warning',
      'منخفض': 'danger'
    };
    return colors[level] || 'info';
  }

  getStatusColor(status: string): string {
    const colors: { [key: string]: string } = {
      'Completed': 'success',
      'In Progress': 'warning',
      'Pending': 'info',
      'Overdue': 'danger',
      'مكتمل': 'success',
      'قيد التنفيذ': 'warning',
      'معلق': 'info',
      'متأخر': 'danger'
    };
    return colors[status] || 'secondary';
  }

  getProgressPercentage(completed: number, total: number): number {
    return total > 0 ? Math.round((completed / total) * 100) : 0;
  }

  // Mock data for fallback
  private getMockOverview(): DashboardOverviewDto {
    return {
      activeAssessments: 5,
      totalControls: 150,
      completedControls: 95,
      overdueControls: 8,
      averageScore: 85.5,
      complianceLevel: 'عالي',
      upcomingDeadlines: [
        { name: 'تقييم الأمن السيبراني', dueDate: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString(), daysRemaining: 7 },
        { name: 'مراجعة الامتثال SAMA', dueDate: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000).toISOString(), daysRemaining: 14 }
      ]
    };
  }

  private getMockControls(): MyControlDto[] {
    return [
      {
        id: '1',
        controlName: 'مراجعة سياسات الأمن',
        frameworkName: 'SAMA',
        status: 'قيد التنفيذ',
        dueDate: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000).toISOString(),
        assignedDate: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000).toISOString()
      },
      {
        id: '2',
        controlName: 'تقييم المخاطر',
        frameworkName: 'NCA',
        status: 'معلق',
        dueDate: new Date(Date.now() + 10 * 24 * 60 * 60 * 1000).toISOString(),
        assignedDate: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000).toISOString()
      }
    ];
  }

  private getMockFrameworkProgress(): FrameworkProgressDto[] {
    return [
      {
        frameworkName: 'SAMA',
        totalControls: 50,
        completedControls: 35,
        inProgressControls: 10,
        notStartedControls: 5,
        compliancePercentage: 70
      },
      {
        frameworkName: 'NCA ECC',
        totalControls: 100,
        completedControls: 60,
        inProgressControls: 30,
        notStartedControls: 10,
        compliancePercentage: 60
      }
    ];
  }

  getDaysRemainingText(days: number): string {
    if (days === 0) return 'اليوم';
    if (days === 1) return 'غداً';
    if (days === 2) return 'بعد يومين';
    return `${days} أيام`;
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('ar-SA').format(date);
  }
}

