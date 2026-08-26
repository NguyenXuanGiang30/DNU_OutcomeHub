import React, { useState } from 'react';
import {
  FileBarChart,
  Download,
  Award,
  TrendingUp,
  FileCheck2,
  CheckCircle,
  FileText,
  Calendar,
} from 'lucide-react';
import {
  ResponsiveContainer,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  Tooltip,
  CartesianGrid,
  Legend,
} from 'recharts';

export const ReportsAccreditationPage: React.FC = () => {
  const [selectedStandard, setSelectedStandard] = useState<'AUN-QA' | 'ABET' | 'MOET'>('AUN-QA');

  const cohortTrendData = [
    { ploCode: 'PLO1', k15: 82.0, k16: 85.5, k17: 88.5 },
    { ploCode: 'PLO2', k15: 76.0, k16: 79.0, k17: 82.0 },
    { ploCode: 'PLO3', k15: 84.0, k16: 86.5, k17: 89.2 },
    { ploCode: 'PLO4', k15: 88.0, k16: 89.5, k17: 91.0 },
    { ploCode: 'PLO5', k15: 68.0, k16: 71.0, k17: 74.5 },
    { ploCode: 'PLO6', k15: 90.0, k16: 92.5, k17: 94.0 },
  ];

  return (
    <div className="animate-fade-in">
      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            Báo Cáo Kiểm Định Chất Lượng & Phân Tích Xu Hướng (Mục 8.6)
          </h2>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', marginTop: '0.25rem' }}>
            Tự động sinh báo cáo tự đánh giá (Self-Study Report) theo tiêu chuẩn AUN-QA, ABET, MOET và đối sánh tiến trình qua các niên khóa.
          </p>
        </div>

        <div style={{ display: 'flex', gap: '0.75rem' }}>
          <button className="btn btn-secondary">
            <Download size={16} />
            <span>Xuất Hồ Sơ Minh Chứng (.ZIP)</span>
          </button>
          <button className="btn btn-primary">
            <FileText size={16} />
            <span>Xuất Báo Cáo Tự Đánh Giá (PDF)</span>
          </button>
        </div>
      </div>

      {/* Standard Selector Filter */}
      <div style={{ display: 'flex', gap: '0.75rem', marginBottom: '1.5rem' }}>
        {(['AUN-QA', 'ABET', 'MOET'] as const).map((std) => (
          <button
            key={std}
            onClick={() => setSelectedStandard(std)}
            className={`btn ${selectedStandard === std ? 'btn-primary' : 'btn-secondary'}`}
            style={{ fontSize: '0.875rem', padding: '0.5rem 1.25rem' }}
          >
            <Award size={16} />
            <span>Chuẩn Kiểm Định {std}</span>
          </button>
        ))}
      </div>

      {/* Overview Cards */}
      <div className="grid-cols-3">
        <div className="glass-card">
          <span className="form-label">Mức Độ Tuân Thủ Tiêu Chuẩn {selectedStandard}</span>
          <span className="kpi-value" style={{ background: 'var(--emerald-gradient)', WebkitBackgroundClip: 'text' }}>
            94.5%
          </span>
          <div className="kpi-trend positive">
            <CheckCircle size={14} />
            <span>Đạt 5.2/7.0 điểm chuẩn kiểm định quốc tế</span>
          </div>
        </div>

        <div className="glass-card">
          <span className="form-label">Chuẩn Đầu Ra Đạt Ngưỡng Mục Tiêu</span>
          <span className="kpi-value">5 / 6 PLO</span>
          <div className="kpi-trend positive">
            <span>83.3% số lượng CĐR vượt ngưỡng</span>
          </div>
        </div>

        <div className="glass-card">
          <span className="form-label">Vòng Cải Tiến Chất Lượng CQI Hoàn Thành</span>
          <span className="kpi-value" style={{ background: 'var(--cyan-gradient)', WebkitBackgroundClip: 'text' }}>
            4 Chu Kỳ
          </span>
          <div className="kpi-trend positive">
            <span>Khép kín vòng lặp Continuous Improvement</span>
          </div>
        </div>
      </div>

      {/* Historical Trend Bar Chart across Cohorts */}
      <div className="glass-card" style={{ marginTop: '1.5rem' }}>
        <div className="glass-card-header">
          <div>
            <h3 className="glass-card-title">
              <TrendingUp size={20} className="text-primary-400" />
              Đối Sánh Tiến Trình Đạt CĐR Qua 3 Khóa Liên Tiếp (K15 - K16 - K17)
            </h3>
            <p className="glass-card-subtitle">
              Đánh giá hiệu quả của các hành động cải tiến chất lượng CQI theo từng năm học
            </p>
          </div>
        </div>

        <div style={{ width: '100%', height: '340px' }}>
          <ResponsiveContainer>
            <BarChart data={cohortTrendData}>
              <CartesianGrid strokeDasharray="3 3" stroke="rgba(255, 255, 255, 0.08)" />
              <XAxis dataKey="ploCode" stroke="var(--text-secondary)" tick={{ fill: 'var(--text-primary)', fontWeight: 700 }} />
              <YAxis domain={[0, 100]} stroke="var(--text-muted)" />
              <Tooltip contentStyle={{ backgroundColor: 'var(--bg-surface-elevated)', borderColor: 'var(--border-strong)', borderRadius: '8px', color: '#fff' }} />
              <Legend wrapperStyle={{ paddingTop: '10px' }} />
              <Bar dataKey="k15" name="Khóa K15 (2021-2022)" fill="#64748b" radius={[4, 4, 0, 0]} />
              <Bar dataKey="k16" name="Khóa K16 (2022-2023)" fill="#06b6d4" radius={[4, 4, 0, 0]} />
              <Bar dataKey="k17" name="Khóa K17 (2023-2024)" fill="#6366f1" radius={[4, 4, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>
    </div>
  );
};
