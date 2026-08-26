import React, { useState } from 'react';
import { useLocation } from 'react-router-dom';
import {
  Network,
  GitBranch,
  Layers,
  CheckCircle,
  AlertCircle,
  Sparkles,
  Download,
  Share2,
  BookOpen,
  Target,
  Award,
  Hash,
  Scale,
  FileCheck2,
} from 'lucide-react';

interface MatrixCell {
  courseCode: string;
  courseName: string;
  semester: number;
  plos: Record<string, 'I' | 'R' | 'M' | 'A' | 'RA' | 'MA' | '-'>;
}

export const CurriculumMatrixPage: React.FC = () => {
  const location = useLocation();

  const getSubSection = () => {
    if (location.pathname.includes('/curriculum/programs')) return 'programs';
    if (location.pathname.includes('/curriculum/versions')) return 'versions';
    if (location.pathname.includes('/curriculum/pos')) return 'pos';
    if (location.pathname.includes('/curriculum/plos')) return 'plos';
    if (location.pathname.includes('/curriculum/pis')) return 'pis';
    if (location.pathname.includes('/curriculum/weight-a')) return 'weight-a';
    if (location.pathname.includes('/curriculum/clos')) return 'clos';
    return 'matrix';
  };

  const [activeTab, setActiveTab] = useState<string>(getSubSection());

  // Matrix Sample Dataset
  const matrixData: MatrixCell[] = [
    { courseCode: 'IT1101', courseName: 'Nhập môn Lập trình', semester: 1, plos: { PLO1: 'I', PLO2: 'I', PLO3: 'I', PLO4: '-', PLO5: '-', PLO6: 'I' } },
    { courseCode: 'IT2102', courseName: 'Cấu trúc Dữ liệu & Giải thuật', semester: 2, plos: { PLO1: 'R', PLO2: 'R', PLO3: 'R', PLO4: '-', PLO5: 'I', PLO6: '-' } },
    { courseCode: 'IT3101', courseName: 'Cơ sở Dữ liệu & SQL', semester: 3, plos: { PLO1: 'R', PLO2: 'R', PLO3: 'R', PLO4: 'I', PLO5: 'I', PLO6: '-' } },
    { courseCode: 'IT3202', courseName: 'Phân tích & Thiết kế HTTT', semester: 4, plos: { PLO1: 'R', PLO2: 'M', PLO3: 'R', PLO4: 'R', PLO5: 'R', PLO6: 'R' } },
    { courseCode: 'IT4101', courseName: 'Lập trình .NET Nâng cao', semester: 5, plos: { PLO1: 'M', PLO2: 'M', PLO3: 'A', PLO4: 'R', PLO5: 'RA', PLO6: 'R' } },
    { courseCode: 'IT4205', courseName: 'Kiểm thử Phần mềm & QA', semester: 6, plos: { PLO1: 'M', PLO2: 'R', PLO3: 'R', PLO4: 'R', PLO5: 'MA', PLO6: 'R' } },
    { courseCode: 'IT4999', courseName: 'Khóa luận Tốt nghiệp', semester: 8, plos: { PLO1: 'A', PLO2: 'A', PLO3: 'MA', PLO4: 'MA', PLO5: 'MA', PLO6: 'MA' } },
  ];

  return (
    <div className="animate-fade-in">
      {/* Header Banner */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            Chương Trình Đào Tạo & Chuẩn Đầu Ra (Mục 8.2)
          </h2>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', marginTop: '0.25rem' }}>
            Quản lý cấu trúc PO, PLO, PI, Trọng số A, CLO và Ma trận 2 chiều I/R/M/A theo từng phiên bản CTĐT & Khóa.
          </p>
        </div>

        <div style={{ display: 'flex', gap: '0.75rem' }}>
          <button className="btn btn-secondary">
            <Download size={16} />
            <span>Xuất Ma Trận Excel</span>
          </button>
          <button className="btn btn-primary">
            <Sparkles size={16} />
            <span>AI Chẩn Đoán Ma Trận</span>
          </button>
        </div>
      </div>

      {/* Navigation Sub-Tabs */}
      <div style={{ display: 'flex', gap: '0.5rem', marginBottom: '1.5rem', borderBottom: '1px solid var(--border-medium)', paddingBottom: '0.5rem', overflowX: 'auto' }}>
        {[
          { key: 'matrix', label: 'Ma Trận Liên Kết (I/R/M/A)', icon: Network },
          { key: 'versions', label: 'Phiên Bản CTĐT', icon: BookOpen },
          { key: 'pos', label: 'Mục Tiêu Đào Tạo (PO)', icon: Target },
          { key: 'plos', label: 'Chuẩn Đầu Ra (PLO1 - PLO9)', icon: Award },
          { key: 'pis', label: 'Chỉ Báo Thực Hiện (PI)', icon: Hash },
          { key: 'weight-a', label: 'Trọng Số Đo Trực Tiếp A', icon: Scale },
          { key: 'clos', label: 'Chuẩn Đầu Ra Học Phần (CLO)', icon: FileCheck2 },
          { key: 'prerequisites', label: 'Cây Tiên Quyết (DAG)', icon: GitBranch },
        ].map((tab) => (
          <button
            key={tab.key}
            onClick={() => setActiveTab(tab.key)}
            className={`btn ${activeTab === tab.key ? 'btn-primary' : 'btn-secondary'}`}
            style={{ fontSize: '0.8125rem' }}
          >
            <tab.icon size={16} />
            <span>{tab.label}</span>
          </button>
        ))}
      </div>

      {/* Scope Selector Bar */}
      <div className="glass-card" style={{ marginBottom: '1.25rem', padding: '0.875rem 1.25rem' }}>
        <div style={{ display: 'flex', gap: '1.5rem', alignItems: 'center', flexWrap: 'wrap' }}>
          <div>
            <span className="form-label">Ngành Đào Tạo</span>
            <select className="form-select" style={{ width: '220px', marginTop: '0.25rem' }}>
              <option>Kỹ thuật Phần mềm (7480201)</option>
              <option>Khoa học Máy tính (7480101)</option>
            </select>
          </div>
          <div>
            <span className="form-label">Phiên Bản CTĐT</span>
            <select className="form-select" style={{ width: '240px', marginTop: '0.25rem' }}>
              <option>KTPM v2023 - Áp dụng K17 (9 PLO ABET)</option>
              <option>KTPM v2022 - Áp dụng K16 (6 PLO)</option>
              <option>KTPM v2021 - Áp dụng K15 (6 PLO)</option>
            </select>
          </div>
          <div>
            <span className="form-label">Trạng Thái CTĐT</span>
            <div style={{ marginTop: '0.5rem' }}>
              <span className="badge badge-success">ĐANG ÁP DỤNG (ACTIVE & LOCKED)</span>
            </div>
          </div>
        </div>
      </div>

      {/* TAB 1: MA TRẬN 2D (I/R/M/A) */}
      {activeTab === 'matrix' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Ma Trận Đóng Góp CĐR Của Các Học Phần (CTĐT K17 - 145 Tín chỉ)</h3>
              <p className="glass-card-subtitle">
                Quy ước: <strong>I</strong> = Introduce, <strong>R</strong> = Reinforce, <strong>M</strong> = Master, <strong>A</strong> = Direct Assessment (Đo trực tiếp)
              </p>
            </div>
            <div style={{ display: 'flex', gap: '0.5rem' }}>
              <span className="badge badge-primary">I: Giới thiệu</span>
              <span className="badge badge-cyan">R: Củng cố</span>
              <span className="badge badge-success">M: Thuần thục</span>
              <span className="badge badge-danger">A: Đo trực tiếp</span>
            </div>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Môn</th>
                  <th>Tên Học Phần</th>
                  <th>Học Kỳ</th>
                  <th style={{ textAlign: 'center' }}>PLO1 (Kiến thức)</th>
                  <th style={{ textAlign: 'center' }}>PLO2 (Thiết kế)</th>
                  <th style={{ textAlign: 'center' }}>PLO3 (Lập trình)</th>
                  <th style={{ textAlign: 'center' }}>PLO4 (Kỹ năng mềm)</th>
                  <th style={{ textAlign: 'center' }}>PLO5 (Kiểm thử)</th>
                  <th style={{ textAlign: 'center' }}>PLO6 (Đạo đức)</th>
                </tr>
              </thead>
              <tbody>
                {matrixData.map((row) => (
                  <tr key={row.courseCode}>
                    <td><strong className="badge badge-secondary">{row.courseCode}</strong></td>
                    <td style={{ fontWeight: 600 }}>{row.courseName}</td>
                    <td>HK {row.semester}</td>
                    {['PLO1', 'PLO2', 'PLO3', 'PLO4', 'PLO5', 'PLO6'].map((plo) => {
                      const val = row.plos[plo] || '-';
                      let badgeClass = 'badge-secondary';
                      if (val.includes('A')) badgeClass = 'badge-danger';
                      else if (val === 'M') badgeClass = 'badge-success';
                      else if (val === 'R') badgeClass = 'badge-cyan';
                      else if (val === 'I') badgeClass = 'badge-primary';

                      return (
                        <td key={plo} style={{ textAlign: 'center' }}>
                          {val !== '-' ? (
                            <span className={`badge ${badgeClass}`} style={{ minWidth: '38px', justifyContent: 'center', fontWeight: 800 }}>
                              {val}
                            </span>
                          ) : (
                            <span style={{ color: 'var(--text-muted)' }}>-</span>
                          )}
                        </td>
                      );
                    })}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 2: PHIÊN BẢN CTĐT */}
      {activeTab === 'versions' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Quản Lý Lịch Sử Các Phiên Bản CTĐT Ngành KTPM</h3>
              <p className="glass-card-subtitle">Hỗ trợ áp dụng độc lập cho từng Khóa mà không bị xung đột</p>
            </div>
            <button className="btn btn-sm btn-primary">+ Tạo Phiên Bản CTĐT Mới</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Phiên Bản</th>
                  <th>Khóa Áp Dụng</th>
                  <th>Số Tín Chỉ</th>
                  <th>Khung CĐR</th>
                  <th>Ngày Ban Hành</th>
                  <th>Trạng Thái</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><strong>KTPM_2024_v3</strong></td>
                  <td>Khóa 18 (2024 - 2028)</td>
                  <td>145 TC</td>
                  <td><span className="badge badge-primary">9 PLO (ABET CAC)</span></td>
                  <td>15/08/2024</td>
                  <td><span className="badge badge-success">ACTIVE (ĐANG DÙNG)</span></td>
                </tr>
                <tr>
                  <td><strong>KTPM_2023_v2</strong></td>
                  <td>Khóa 17 (2023 - 2027)</td>
                  <td>145 TC</td>
                  <td><span className="badge badge-primary">9 PLO (ABET CAC)</span></td>
                  <td>10/08/2023</td>
                  <td><span className="badge badge-success">ACTIVE & LOCKED</span></td>
                </tr>
                <tr>
                  <td><strong>KTPM_2021_v1</strong></td>
                  <td>Khóa 15, Khóa 16</td>
                  <td>140 TC</td>
                  <td><span className="badge badge-secondary">6 PLO (AUN-QA)</span></td>
                  <td>05/08/2021</td>
                  <td><span className="badge badge-secondary">LOCKED</span></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 3: MỤC TIÊU ĐÀO TẠO (PO) */}
      {activeTab === 'pos' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Mục Tiêu Đào Tạo (Program Objectives - PO)</h3>
              <p className="glass-card-subtitle">Định hướng năng lực sinh viên sau 3 - 5 năm tốt nghiệp</p>
            </div>
            <button className="btn btn-sm btn-primary">+ Thêm Mục Tiêu PO</button>
          </div>

          <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
            {[
              { code: 'PO1', title: 'Năng Lực Chuyên Môn & Kỹ Thuật', desc: 'Có khả năng phân tích, thiết kế, hiện thực hóa và kiểm thử các hệ thống phần mềm phức tạp trong môi trường công nghiệp.' },
              { code: 'PO2', title: 'Khả Năng Lãnh Đạo & Làm Việc Nhóm', desc: 'Có kỹ năng giao tiếp hiệu quả, làm việc nhóm đa văn hóa, và đảm nhiệm vai trò quản lý dự án phần mềm.' },
              { code: 'PO3', title: 'Học Tập Suốt Đời & Trách Nhiệm Xã Hội', desc: 'Có tinh thần tự học, thích ứng với công nghệ mới (AI, Cloud), tuân thủ đạo đức nghề nghiệp và pháp luật.' },
            ].map((po) => (
              <div key={po.code} style={{ padding: '1.25rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.35rem' }}>
                  <strong style={{ color: 'var(--primary-400)', fontSize: '1rem' }}>{po.code}: {po.title}</strong>
                  <span className="badge badge-primary">Mục tiêu cấp ngành</span>
                </div>
                <p style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>{po.desc}</p>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* TAB 4: CHUẨN ĐẦU RA (PLO1 - PLO9) */}
      {activeTab === 'plos' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Hệ Thống Chuẩn Đầu Ra (PLO1 – PLO4 Cấp Trường, PLO5 – PLO9 Cấp Ngành)</h3>
              <p className="glass-card-subtitle">Tuân thủ nghiêm ngặt theo Khung chuẩn đầu ra ĐH Đại Nam và chuẩn kiểm định ABET</p>
            </div>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã PLO</th>
                  <th>Phân Cấp</th>
                  <th>Mô Tả Năng Lực Chuẩn Đầu Ra</th>
                  <th>Mức Bloom</th>
                  <th>Liên Kết PO</th>
                  <th>Ngưỡng Đạt</th>
                </tr>
              </thead>
              <tbody>
                {[
                  { code: 'PLO1', scope: 'CẤP TRƯỜNG', desc: 'Áp dụng kiến thức toán học, khoa học cơ bản và công nghệ thông tin vào thực tiễn', bloom: 'APPLY (Mức 3)', po: 'PO1', target: '80%' },
                  { code: 'PLO2', scope: 'CẤP TRƯỜNG', desc: 'Phân tích vấn đề kỹ thuật phức tạp và áp dụng nguyên lý công nghệ để giải quyết', bloom: 'ANALYZE (Mức 4)', po: 'PO1', target: '80%' },
                  { code: 'PLO3', scope: 'CẤP TRƯỜNG', desc: 'Thiết kế, hiện thực hóa và đánh giá giải pháp phần mềm đáp ứng yêu cầu', bloom: 'CREATE (Mức 6)', po: 'PO1', target: '80%' },
                  { code: 'PLO4', scope: 'CẤP TRƯỜNG', desc: 'Giao tiếp hiệu quả trong môi trường chuyên môn bằng lời nói và văn bản', bloom: 'APPLY (Mức 3)', po: 'PO2', target: '80%' },
                  { code: 'PLO5', scope: 'CẤP NGÀNH', desc: 'Thực hiện kiểm thử phần mềm, đảm bảo chất lượng và an toàn bảo mật hệ thống', bloom: 'EVALUATE (Mức 5)', po: 'PO1', target: '80%' },
                  { code: 'PLO6', scope: 'CẤP NGÀNH', desc: 'Thể hiện trách nhiệm nghề nghiệp và đưa ra quyết định dựa trên đạo đức, pháp luật', bloom: 'EVALUATE (Mức 5)', po: 'PO3', target: '80%' },
                ].map((plo) => (
                  <tr key={plo.code}>
                    <td><strong className="badge badge-primary">{plo.code}</strong></td>
                    <td><span className={`badge ${plo.scope === 'CẤP TRƯỜNG' ? 'badge-cyan' : 'badge-secondary'}`}>{plo.scope}</span></td>
                    <td style={{ fontWeight: 600 }}>{plo.desc}</td>
                    <td><span className="badge badge-bloom badge-cyan">{plo.bloom}</span></td>
                    <td><code>{plo.po}</code></td>
                    <td style={{ color: 'var(--emerald-400)', fontWeight: 700 }}>{plo.target}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 5: CHỈ BÁO THỰC HIỆN (PI) */}
      {activeTab === 'pis' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Danh Mục Chỉ Báo Thực Hiện (Performance Indicators - PI)</h3>
              <p className="glass-card-subtitle">Mỗi PLO được phân rã thành các chỉ báo hành vi có thể đo lường trực tiếp</p>
            </div>
            <button className="btn btn-sm btn-primary">+ Thêm Chỉ Báo PI</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã PI</th>
                  <th>Thuộc PLO</th>
                  <th>Mô Tả Hành Vi Đo Lường Cụ Thể</th>
                  <th>Mức Bloom</th>
                  <th>Học Phần Đảm Nhận Đo (A)</th>
                </tr>
              </thead>
              <tbody>
                {[
                  { pi: 'PI 1.1', plo: 'PLO1', desc: 'Vận dụng được kiến thức cấu trúc dữ liệu và giải thuật trong giải quyết bài toán', bloom: 'APPLY (3)', course: 'IT2102 Cấu trúc dữ liệu' },
                  { pi: 'PI 2.1', plo: 'PLO2', desc: 'Phân tích và mô hình hóa yêu cầu hệ thống phần mềm bằng UML/BPMN', bloom: 'ANALYZE (4)', course: 'IT3202 Phân tích thiết kế' },
                  { pi: 'PI 3.1', plo: 'PLO3', desc: 'Xây dựng được ứng dụng Web API hoàn chỉnh có tích hợp cơ sở dữ liệu', bloom: 'CREATE (6)', course: 'IT4101 Lập trình .NET' },
                  { pi: 'PI 5.1', plo: 'PLO5', desc: 'Thiết kế Unit Test case và tự động hóa quy trình kiểm thử CI/CD', bloom: 'EVALUATE (5)', course: 'IT4101 & IT4205' },
                ].map((item) => (
                  <tr key={item.pi}>
                    <td><strong className="badge badge-cyan">{item.pi}</strong></td>
                    <td><code>{item.plo}</code></td>
                    <td style={{ fontWeight: 600 }}>{item.desc}</td>
                    <td><span className="badge badge-bloom badge-secondary">{item.bloom}</span></td>
                    <td><span className="badge badge-danger">{item.course}</span></td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 6: TRỌNG SỐ A */}
      {activeTab === 'weight-a' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Bảng Khai Báo Trọng Số Đo Trực Tiếp A Theo CTĐT</h3>
              <p className="glass-card-subtitle">Tổng trọng số đo trực tiếp của các học phần cho mỗi PI phải đạt chuẩn 100%</p>
            </div>
            <span className="badge badge-success">Tổng kiểm tra: 100% Hợp lệ</span>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Chỉ Số PI</th>
                  <th>Học Phần Đảm Nhận Đo (A)</th>
                  <th>Bài Đánh Giá Đo Lường</th>
                  <th>Trọng Số Trong Học Phần</th>
                  <th>Trọng Số Đóng Góp Vào PI</th>
                  <th>Kiểm Tra Tổng PI</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td rowSpan={2} style={{ verticalAlign: 'middle', fontWeight: 800, color: 'var(--primary-400)' }}>PI 3.1</td>
                  <td>IT4101: Lập trình .NET</td>
                  <td>A2: Bài Thực Hành Web API</td>
                  <td>40.0%</td>
                  <td><strong>60.0%</strong></td>
                  <td rowSpan={2} style={{ verticalAlign: 'middle', textAlign: 'center' }}>
                    <span className="badge badge-success">Tổng: 100.0% ✓</span>
                  </td>
                </tr>
                <tr>
                  <td>IT4999: Khóa luận Tốt nghiệp</td>
                  <td>A3: Báo cáo & Demo Đồ án</td>
                  <td>50.0%</td>
                  <td><strong>40.0%</strong></td>
                </tr>

                <tr>
                  <td rowSpan={2} style={{ verticalAlign: 'middle', fontWeight: 800, color: 'var(--primary-400)' }}>PI 5.1</td>
                  <td>IT4101: Lập trình .NET</td>
                  <td>A2: Unit Testing Module</td>
                  <td>50.0%</td>
                  <td><strong>50.0%</strong></td>
                  <td rowSpan={2} style={{ verticalAlign: 'middle', textAlign: 'center' }}>
                    <span className="badge badge-success">Tổng: 100.0% ✓</span>
                  </td>
                </tr>
                <tr>
                  <td>IT4205: Kiểm thử Phần mềm</td>
                  <td>A3: Báo cáo Kiểm thử Tự động</td>
                  <td>50.0%</td>
                  <td><strong>50.0%</strong></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 7: CHUẨN ĐẦU RA HỌC PHẦN (CLO) */}
      {activeTab === 'clos' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Danh Mục Chuẩn Đầu Ra Học Phần (CLO)</h3>
              <p className="glass-card-subtitle">Ánh xạ từ CLO môn học lên chỉ số PI và PLO của chương trình</p>
            </div>
            <select className="form-select" style={{ width: '260px' }}>
              <option>IT4101 - Lập trình .NET Nâng cao</option>
              <option>IT2102 - Cấu trúc Dữ liệu & Giải thuật</option>
            </select>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã CLO</th>
                  <th>Mô Tả Chuẩn Đầu Ra Môn Học</th>
                  <th>Mức Bloom</th>
                  <th>Ánh Xạ Tới Chỉ Số PI</th>
                  <th>Bài Đánh Giá</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><strong className="badge badge-cyan">CLO1</strong></td>
                  <td>Xây dựng RESTful API và xử lý dữ liệu với Entity Framework Core</td>
                  <td><span className="badge badge-bloom badge-cyan">APPLY (Mức 3)</span></td>
                  <td><strong>PI 3.1</strong></td>
                  <td>Bài Thực Hành A2</td>
                </tr>
                <tr>
                  <td><strong className="badge badge-cyan">CLO2</strong></td>
                  <td>Áp dụng kiến trúc Clean Architecture và Dependency Injection</td>
                  <td><span className="badge badge-bloom badge-cyan">ANALYZE (Mức 4)</span></td>
                  <td><strong>PI 2.1</strong></td>
                  <td>Đồ Án A3</td>
                </tr>
                <tr>
                  <td><strong className="badge badge-cyan">CLO3</strong></td>
                  <td>Viết Unit Test và kiểm thử bảo mật cho các API endpoint</td>
                  <td><span className="badge badge-bloom badge-cyan">EVALUATE (Mức 5)</span></td>
                  <td><strong>PI 5.1</strong></td>
                  <td>Bài Thực Hành A2 & Đồ Án A3</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 8: CÂY TIÊN QUYẾT (DAG) */}
      {activeTab === 'prerequisites' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Sơ Đồ Đồ Thị Tiên Quyết Học Phần (Prerequisite DAG)</h3>
              <p className="glass-card-subtitle">Đường găng dẫn đến Khóa luận Tốt nghiệp</p>
            </div>
          </div>

          <div style={{ padding: '2rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)', textAlign: 'center' }}>
            <div style={{ display: 'flex', gap: '1.5rem', alignItems: 'center', justifyContent: 'center', flexWrap: 'wrap', marginBottom: '1.5rem' }}>
              <div style={{ padding: '0.75rem 1.25rem', backgroundColor: 'rgba(99, 102, 241, 0.2)', border: '1px solid var(--primary-500)', borderRadius: 'var(--radius-md)' }}>
                <strong>IT1101</strong><br /><span style={{ fontSize: '0.75rem' }}>Nhập môn Lập trình</span>
              </div>
              <span style={{ color: 'var(--primary-400)', fontWeight: 800 }}>➔</span>
              <div style={{ padding: '0.75rem 1.25rem', backgroundColor: 'rgba(6, 182, 212, 0.2)', border: '1px solid var(--cyan-500)', borderRadius: 'var(--radius-md)' }}>
                <strong>IT2102</strong><br /><span style={{ fontSize: '0.75rem' }}>Cấu trúc Dữ liệu</span>
              </div>
              <span style={{ color: 'var(--primary-400)', fontWeight: 800 }}>➔</span>
              <div style={{ padding: '0.75rem 1.25rem', backgroundColor: 'rgba(16, 185, 129, 0.2)', border: '1px solid var(--emerald-500)', borderRadius: 'var(--radius-md)' }}>
                <strong>IT4101</strong><br /><span style={{ fontSize: '0.75rem' }}>Lập trình .NET</span>
              </div>
              <span style={{ color: 'var(--primary-400)', fontWeight: 800 }}>➔</span>
              <div style={{ padding: '0.75rem 1.25rem', backgroundColor: 'rgba(244, 63, 94, 0.2)', border: '1px solid var(--rose-500)', borderRadius: 'var(--radius-md)' }}>
                <strong>IT4999</strong><br /><span style={{ fontSize: '0.75rem' }}>Khóa luận Tốt nghiệp</span>
              </div>
            </div>
            <p style={{ fontSize: '0.8125rem', color: 'var(--text-secondary)' }}>
              Đường găng: <strong>IT1101 ➔ IT2102 ➔ IT4101 ➔ IT4999</strong> (Tổng 4 học kỳ tích lũy).
            </p>
          </div>
        </div>
      )}
    </div>
  );
};
