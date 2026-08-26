import React, { useState, useEffect } from 'react';
import {
  Users,
  Calendar,
  Award,
  AlertTriangle,
  TrendingUp,
  Filter,
  Sparkles,
  Plus,
} from 'lucide-react';
import {
  ResponsiveContainer,
  RadarChart,
  PolarGrid,
  PolarAngleAxis,
  PolarRadiusAxis,
  Radar,
  Tooltip,
} from 'recharts';
import { reportsApi, DashboardResponseDto } from '../api/reportsApi';
import { EmptyState } from '../components/common/EmptyState';
import { useNavigate } from 'react-router-dom';

export const DashboardPage: React.FC = () => {
  const navigate = useNavigate();
  const [data, setData] = useState<DashboardResponseDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    reportsApi
      .getDashboardData()
      .then((res) => setData(res))
      .catch((err) => {
        console.error('Failed to load dashboard:', err);
        // Clean default for empty database
        setData({
          metrics: {
            totalStudents: 0,
            totalActivePeriods: 0,
            totalCoursesAssessed: 0,
            overallPloAttainmentRate: 0,
            pendingCqiPlansCount: 0,
            lastDataRefreshTime: new Date().toISOString(),
          },
          ploRadar: [],
          earlyWarnings: [],
        });
      })
      .finally(() => setIsLoading(false));
  }, []);

  return (
    <div className="animate-fade-in">
      {/* Header Title Banner */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            Bảng Điều Khiển Chuẩn Đầu Ra (OBE Executive Dashboard)
          </h2>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', marginTop: '0.25rem' }}>
            Theo dõi thời gian thực mức độ đạt chuẩn PLO, chỉ số PI và cảnh báo rủi ro sớm theo chuẩn kiểm định ABET / AUN-QA.
          </p>
        </div>

        <div style={{ display: 'flex', gap: '0.75rem' }}>
          <button className="btn btn-secondary">
            <Filter size={16} />
            <span>Năm học hiện tại</span>
          </button>
          <button onClick={() => navigate('/data/org-units')} className="btn btn-primary">
            <Plus size={16} />
            <span>Khởi Tạo Dữ Liệu Đào Tạo</span>
          </button>
        </div>
      </div>

      {/* KPI Cards Row (4 Cards) */}
      <div className="grid-cols-4">
        <div className="glass-card kpi-card">
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <span className="form-label">Tổng Sinh Viên Đo Lường</span>
            <span style={{ padding: '0.4rem', borderRadius: 'var(--radius-sm)', background: 'rgba(99, 102, 241, 0.15)', color: 'var(--primary-400)' }}>
              <Users size={18} />
            </span>
          </div>
          <span className="kpi-value">{data?.metrics.totalStudents ?? 0}</span>
          <div className="kpi-trend positive"><span>Dữ liệu mới nhất</span></div>
        </div>

        <div className="glass-card kpi-card">
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <span className="form-label">Tỷ Lệ Đạt CĐR Toàn Trường</span>
            <span style={{ padding: '0.4rem', borderRadius: 'var(--radius-sm)', background: 'rgba(16, 185, 129, 0.15)', color: 'var(--emerald-400)' }}>
              <Award size={18} />
            </span>
          </div>
          <span className="kpi-value" style={{ background: 'var(--emerald-gradient)', WebkitBackgroundClip: 'text' }}>
            {data?.metrics.overallPloAttainmentRate ?? 0}%
          </span>
          <div className="kpi-trend positive"><span>Ngưỡng kỳ vọng: ≥ 80%</span></div>
        </div>

        <div className="glass-card kpi-card">
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <span className="form-label">Học Phần Đã Đánh Giá (A)</span>
            <span style={{ padding: '0.4rem', borderRadius: 'var(--radius-sm)', background: 'rgba(6, 182, 212, 0.15)', color: 'var(--cyan-400)' }}>
              <TrendingUp size={18} />
            </span>
          </div>
          <span className="kpi-value">{data?.metrics.totalCoursesAssessed ?? 0}</span>
          <div className="kpi-trend positive"><span>Học phần đảm nhận đo</span></div>
        </div>

        <div className="glass-card kpi-card">
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <span className="form-label">Kế Hoạch CQI Đang Mở</span>
            <span style={{ padding: '0.4rem', borderRadius: 'var(--radius-sm)', background: 'rgba(245, 158, 11, 0.15)', color: 'var(--amber-400)' }}>
              <Calendar size={18} />
            </span>
          </div>
          <span className="kpi-value">{data?.metrics.pendingCqiPlansCount ?? 0}</span>
          <div className="kpi-trend positive"><span>Vòng lặp cải tiến</span></div>
        </div>
      </div>

      {/* Main Visuals: Radar Chart + Early Warnings Drilldown */}
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.5rem', marginTop: '1.5rem' }}>
        {/* Radar Chart Card */}
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">
                <Award size={20} className="text-primary-400" />
                Biểu Đồ Radar Mức Độ Đạt Chuẩn Đầu Ra (PLO)
              </h3>
              <p className="glass-card-subtitle">
                Đường nét đứt thể hiện ngưỡng mục tiêu chuẩn đầu ra (80%)
              </p>
            </div>
          </div>

          {!data || data.ploRadar.length === 0 ? (
            <EmptyState
              title="Chưa có dữ liệu biểu đồ Radar"
              description="Hiện tại chưa có đợt đo và điểm tính toán CĐR nào để hiển thị biểu đồ."
              actionLabel="Đi Tới Quản Lý Đào Tạo"
              onAction={() => navigate('/data/org-units')}
            />
          ) : (
            <div style={{ width: '100%', height: '360px' }}>
              <ResponsiveContainer>
                <RadarChart data={data.ploRadar} margin={{ top: 20, right: 30, bottom: 20, left: 30 }}>
                  <PolarGrid stroke="rgba(255, 255, 255, 0.12)" strokeDasharray="3 3" />
                  <PolarAngleAxis dataKey="ploCode" stroke="var(--text-secondary)" tick={{ fill: 'var(--text-primary)', fontSize: 12, fontWeight: 700 }} />
                  <PolarRadiusAxis angle={30} domain={[0, 100]} stroke="rgba(255, 255, 255, 0.2)" />
                  <Radar name="Tỷ lệ đạt thực tế (%)" dataKey="attainmentPercentage" stroke="#6366f1" fill="#6366f1" fillOpacity={0.4} strokeWidth={2} />
                  <Radar name="Ngưỡng mục tiêu (%)" dataKey="targetThresholdPercentage" stroke="#10b981" fill="none" strokeWidth={2} strokeDasharray="4 4" />
                  <Tooltip contentStyle={{ backgroundColor: 'var(--bg-surface-elevated)', borderColor: 'var(--border-strong)', borderRadius: '8px', color: '#fff' }} />
                </RadarChart>
              </ResponsiveContainer>
            </div>
          )}
        </div>

        {/* Early Warning Card */}
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">
                <AlertTriangle size={20} className="text-amber-400" />
                Cảnh Báo Sớm & Drill-Down Truy Vết
              </h3>
              <p className="glass-card-subtitle">
                Truy vết chi tiết tới PI, CLO và sinh viên
              </p>
            </div>
          </div>

          {!data || data.earlyWarnings.length === 0 ? (
            <EmptyState
              title="Không có cảnh báo nguy cơ nào"
              description="Hệ thống không ghi nhận nhóm lớp hay chuẩn đầu ra nào dưới ngưỡng cảnh báo."
            />
          ) : (
            <div style={{ display: 'flex', flexDirection: 'column', gap: '0.75rem' }}>
              {data.earlyWarnings.map((item) => (
                <div key={item.groupCode} style={{ padding: '1rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--rose-500)' }}>
                  <strong style={{ color: 'var(--rose-400)' }}>{item.name}</strong>
                  <p style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', marginTop: '0.25rem' }}>
                    Có {item.atRiskStudentCount} sinh viên có nguy cơ chưa đạt
                  </p>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};
