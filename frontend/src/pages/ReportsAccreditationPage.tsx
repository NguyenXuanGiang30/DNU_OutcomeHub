import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import {
  FileBarChart,
  Download,
  Award,
  TrendingUp,
  FileCheck2,
  CheckCircle,
  FileText,
  AlertTriangle,
  Layers,
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
  const location = useLocation();
  const navigate = useNavigate();

  const getSubSection = () => {
    if (location.pathname.includes('/results/clo')) return 'clo';
    if (location.pathname.includes('/results/pi')) return 'pi';
    if (location.pathname.includes('/results/plo')) return 'plo';
    if (location.pathname.includes('/results/warnings')) return 'warnings';
    return 'summary-reports';
  };

  const [activeTab, setActiveTab] = useState<string>(getSubSection());
  const [selectedStandard, setSelectedStandard] = useState<'AUN-QA' | 'ABET' | 'MOET'>('AUN-QA');
  const [toastMessage, setToastMessage] = useState<string | null>(null);

  useEffect(() => {
    setActiveTab(getSubSection());
  }, [location.pathname]);

  const handleTabClick = (key: string) => {
    setActiveTab(key);
    navigate(`/results/${key}`);
  };

  const handleExport = (type: string) => {
    setToastMessage(`✓ Đang xuất ${type}... File sẽ được tải xuống tự động.`);
    setTimeout(() => setToastMessage(null), 3000);
  };

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
      {/* Toast Alert */}
      {toastMessage && (
        <div style={{ position: 'fixed', top: '85px', right: '2rem', zIndex: 100, backgroundColor: 'var(--emerald-500)', color: '#fff', padding: '0.75rem 1.25rem', borderRadius: 'var(--radius-md)', boxShadow: 'var(--glass-shadow)', display: 'flex', alignItems: 'center', gap: '0.5rem', fontWeight: 600 }}>
          <CheckCircle size={18} />
          <span>{toastMessage}</span>
        </div>
      )}

      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            Kết Quả Chuẩn Đầu Ra & Báo Cáo Kiểm Định
          </h2>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', marginTop: '0.25rem' }}>
            Xem chi tiết kết quả CLO, PI, PLO, cảnh báo sinh viên chưa đạt và xuất báo cáo tự đánh giá AUN-QA / ABET / MOET.
          </p>
        </div>

        <div style={{ display: 'flex', gap: '0.75rem' }}>
          <button onClick={() => handleExport('Gói Hồ Sơ Minh Chứng (.ZIP)')} className="btn btn-secondary">
            <Download size={16} />
            <span>Xuất Hồ Sơ Minh Chứng (.ZIP)</span>
          </button>
          <button onClick={() => handleExport('Báo Cáo Tự Đánh Giá (PDF)')} className="btn btn-primary">
            <FileText size={16} />
            <span>Xuất Báo Cáo Tự Đánh Giá (PDF)</span>
          </button>
        </div>
      </div>

      {/* Navigation Sub-Tabs */}
      <div style={{ display: 'flex', gap: '0.5rem', marginBottom: '1.5rem', borderBottom: '1px solid var(--border-medium)', paddingBottom: '0.5rem', overflowX: 'auto' }}>
        {[
          { key: 'summary-reports', label: '1. Báo Cáo Tổng Hợp Kiểm Định', icon: FileBarChart },
          { key: 'plo', label: '2. Kết Quả CĐR (PLO)', icon: Award },
          { key: 'pi', label: '3. Kết Quả Chỉ Báo (PI)', icon: Layers },
          { key: 'clo', label: '4. Kết Quả Môn Học (CLO)', icon: FileCheck2 },
          { key: 'warnings', label: '5. Cảnh Báo Chưa Đạt', icon: AlertTriangle },
        ].map((tab) => (
          <button
            key={tab.key}
            onClick={() => handleTabClick(tab.key)}
            className={`btn ${activeTab === tab.key ? 'btn-primary' : 'btn-secondary'}`}
            style={{ fontSize: '0.8125rem' }}
          >
            <tab.icon size={16} />
            <span>{tab.label}</span>
          </button>
        ))}
      </div>

      {/* TAB: BÁO CÁO TỔNG HỢP */}
      {activeTab === 'summary-reports' && (
        <>
          <div style={{ display: 'flex', gap: '0.75rem', marginBottom: '1.25rem' }}>
            {(['AUN-QA', 'ABET', 'MOET'] as const).map((std) => (
              <button
                key={std}
                onClick={() => setSelectedStandard(std)}
                className={`btn ${selectedStandard === std ? 'btn-primary' : 'btn-secondary'}`}
                style={{ fontSize: '0.875rem' }}
              >
                <Award size={16} />
                <span>Tiêu Chuẩn {std}</span>
              </button>
            ))}
          </div>

          <div className="grid-cols-3">
            <div className="glass-card">
              <span className="form-label">Mức Độ Tuân Thủ Tiêu Chuẩn {selectedStandard}</span>
              <span className="kpi-value" style={{ background: 'var(--emerald-gradient)', WebkitBackgroundClip: 'text' }}>94.5%</span>
              <div className="kpi-trend positive"><CheckCircle size={14} /><span>Đạt 5.2/7.0 điểm kiểm định</span></div>
            </div>
            <div className="glass-card">
              <span className="form-label">CĐR Đạt Ngưỡng Mục Tiêu (80%)</span>
              <span className="kpi-value">5 / 6 PLO</span>
              <div className="kpi-trend positive"><span>83.3% số lượng CĐR vượt ngưỡng</span></div>
            </div>
            <div className="glass-card">
              <span className="form-label">Vòng CQI Hoàn Thành Khép Kín</span>
              <span className="kpi-value" style={{ background: 'var(--cyan-gradient)', WebkitBackgroundClip: 'text' }}>4 Chu Kỳ</span>
              <div className="kpi-trend positive"><span>Đã đo lại & nghiệm thu đóng</span></div>
            </div>
          </div>

          <div className="glass-card" style={{ marginTop: '1.5rem' }}>
            <div className="glass-card-header">
              <div>
                <h3 className="glass-card-title"><TrendingUp size={20} className="text-primary-400" /> Đối Sánh Tiến Trình Đạt CĐR Qua 3 Khóa (K15 - K16 - K17)</h3>
                <p className="glass-card-subtitle">Đánh giá hiệu quả của các hành động cải tiến chất lượng CQI</p>
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
                  <Bar dataKey="k15" name="Khóa K15" fill="#64748b" radius={[4, 4, 0, 0]} />
                  <Bar dataKey="k16" name="Khóa K16" fill="#06b6d4" radius={[4, 4, 0, 0]} />
                  <Bar dataKey="k17" name="Khóa K17" fill="#6366f1" radius={[4, 4, 0, 0]} />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </div>
        </>
      )}

      {/* TAB: KẾT QUẢ PLO */}
      {activeTab === 'plo' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Bảng Tổng Kết Kết Quả Đạt Chuẩn Đầu Ra (PLO1 – PLO6) - Khóa K17</h3>
              <p className="glass-card-subtitle">Tổng hợp từ 28 học phần đo lường</p>
            </div>
          </div>
          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã PLO</th>
                  <th>Tên Chuẩn Đầu Ra</th>
                  <th>Mức Bloom</th>
                  <th>Ngưỡng Mục Tiêu</th>
                  <th>Tỷ Lệ Đạt Thực Tế</th>
                  <th>Đánh Giá</th>
                </tr>
              </thead>
              <tbody>
                {[
                  { code: 'PLO1', name: 'Kiến thức cơ bản & cơ sở ngành', bloom: 'APPLY (3)', target: '80%', actual: '88.5%', met: true },
                  { code: 'PLO2', name: 'Phân tích & thiết kế hệ thống', bloom: 'ANALYZE (4)', target: '80%', actual: '82.0%', met: true },
                  { code: 'PLO3', name: 'Lập trình & hiện thực hóa giải pháp', bloom: 'CREATE (6)', target: '80%', actual: '89.2%', met: true },
                  { code: 'PLO4', name: 'Làm việc nhóm & Giao tiếp', bloom: 'APPLY (3)', target: '80%', actual: '91.0%', met: true },
                  { code: 'PLO5', name: 'Kiểm thử & Đảm bảo chất lượng', bloom: 'EVALUATE (5)', target: '80%', actual: '74.5%', met: false },
                  { code: 'PLO6', name: 'Đạo đức nghề nghiệp & Pháp luật', bloom: 'EVALUATE (5)', target: '80%', actual: '94.0%', met: true },
                ].map((row) => (
                  <tr key={row.code}>
                    <td><strong className="badge badge-primary">{row.code}</strong></td>
                    <td style={{ fontWeight: 600 }}>{row.name}</td>
                    <td><span className="badge badge-bloom badge-cyan">{row.bloom}</span></td>
                    <td>{row.target}</td>
                    <td><strong style={{ color: row.met ? 'var(--emerald-400)' : 'var(--rose-400)' }}>{row.actual}</strong></td>
                    <td><span className={`badge ${row.met ? 'badge-success' : 'badge-danger'}`}>{row.met ? 'ĐẠT CHUẨN' : 'CHƯA ĐẠT'}</span></td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB: KẾT QUẢ PI */}
      {activeTab === 'pi' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Kết Quả Đạt Từng Chỉ Báo Thực Hiện (Performance Indicators - PI)</h3>
              <p className="glass-card-subtitle">Chi tiết mức độ thành thạo của sinh viên ở từng năng lực cụ thể</p>
            </div>
          </div>
          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã PI</th>
                  <th>Thuộc PLO</th>
                  <th>Nội Dung Năng Lực</th>
                  <th>Số SV Tham Gia Đo</th>
                  <th>Tỷ Lệ Đạt (≥ 6.0)</th>
                  <th>Trạng Thái</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><strong className="badge badge-cyan">PI 3.1</strong></td>
                  <td><code>PLO3</code></td>
                  <td>Hiện thực hóa Web API & Database</td>
                  <td>120 SV</td>
                  <td><strong style={{ color: 'var(--emerald-400)' }}>89.2%</strong></td>
                  <td><span className="badge badge-success">ĐẠT CHUẨN</span></td>
                </tr>
                <tr>
                  <td><strong className="badge badge-cyan">PI 5.1</strong></td>
                  <td><code>PLO5</code></td>
                  <td>Viết Unit Test & CI/CD</td>
                  <td>120 SV</td>
                  <td><strong style={{ color: 'var(--rose-400)' }}>74.5%</strong></td>
                  <td><span className="badge badge-danger">CHƯA ĐẠT (CẦN CQI)</span></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB: KẾT QUẢ CLO */}
      {activeTab === 'clo' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Kết Quả Đạt Chuẩn Đầu Ra Học Phần (CLO)</h3>
              <p className="glass-card-subtitle">Học phần: <strong>IT4101 - Lập trình .NET Nâng cao</strong></p>
            </div>
          </div>
          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã CLO</th>
                  <th>Nội Dung Chuẩn Đầu Ra Môn</th>
                  <th>Tỷ Lệ Đạt Lớp 17IT01</th>
                  <th>Tỷ Lệ Đạt Lớp 17IT02</th>
                  <th>Đánh Giá Môn</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><strong className="badge badge-cyan">CLO1</strong></td>
                  <td>Xây dựng RESTful Web API</td>
                  <td>92.5%</td>
                  <td>88.0%</td>
                  <td><span className="badge badge-success">ĐẠT TỐT</span></td>
                </tr>
                <tr>
                  <td><strong className="badge badge-cyan">CLO2</strong></td>
                  <td>Kiến trúc Clean Architecture</td>
                  <td>85.0%</td>
                  <td>81.5%</td>
                  <td><span className="badge badge-success">ĐẠT</span></td>
                </tr>
                <tr>
                  <td><strong className="badge badge-cyan">CLO3</strong></td>
                  <td>Kiểm thử Unit Test</td>
                  <td>75.0%</td>
                  <td>71.0%</td>
                  <td><span className="badge badge-warning">CẦN CẢI TIẾN</span></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB: CẢNH BÁO CHƯA ĐẠT */}
      {activeTab === 'warnings' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title"><AlertTriangle size={20} className="text-amber-400" /> Danh Sách Cảnh Báo Sinh Viên Chưa Đạt Chuẩn Đầu Ra</h3>
              <p className="glass-card-subtitle">Cần can thiệp phụ đạo học vụ và mở kế hoạch cải tiến CQI</p>
            </div>
          </div>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
            <div style={{ padding: '1.25rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--rose-500)' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.5rem' }}>
                <strong style={{ color: 'var(--rose-400)' }}>Cảnh báo mức cao: Lớp 17IT01 - Môn Lập trình .NET</strong>
                <span className="badge badge-danger">6 Sinh Viên Chưa Đạt PI 5.1</span>
              </div>
              <p style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>• Nguyên nhân: Điểm bài thực hành Unit Test dưới 6.0/10.</p>
              <div style={{ marginTop: '0.75rem', display: 'flex', gap: '0.5rem' }}>
                <button onClick={() => navigate('/cqi/action-plans')} className="btn btn-sm btn-primary">Khởi Tạo Kế Hoạch CQI Cho Lớp</button>
                <button className="btn btn-sm btn-secondary">Xem Danh Sách 6 Sinh Viên</button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
