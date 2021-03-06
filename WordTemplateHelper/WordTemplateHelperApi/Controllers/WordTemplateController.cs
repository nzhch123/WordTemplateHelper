using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WordTemplateHelperApi.Data;
using WordTemplateHelperApi.Models;
using Microsoft.AspNetCore.Cors;

namespace WordTemplateHelperApi.Controllers
{
    [Produces("application/json")]
    [Route("api/WordTemplate/[action]")]
    [EnableCors("AllowCrossDomain")]
    public class WordTemplateController : Controller
    {
        private readonly WordTemplateContext _context;

        public WordTemplateController(WordTemplateContext context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<ResponseResultInfo> AddWordTemplate(string userId, string organizationId, TemplateType type, [FromBody]WordTemplateInfo item)
        {
            ResponseResultInfo respResult = new ResponseResultInfo();
            try
            {
                item.Id = Guid.NewGuid().ToString();
                item.CreateTime = DateTime.Now;
                _context.WordTemplateInfoes.Add(item);
                //根据类型添加到不同的数据库
                switch (type)
                {
                    case TemplateType.Private:
                        PrivateTemplateInfo priTmp = new PrivateTemplateInfo();
                        priTmp.CreateTime = DateTime.Now;
                        priTmp.Id = Guid.NewGuid().ToString();
                        priTmp.TemplateId = item.Id;
                        priTmp.UserId = userId;
                        await _context.PrivateTemplateInfoes.AddAsync(priTmp);
                        break;
                    case TemplateType.Public:
                        break;
                    case TemplateType.Organization:
                        OrganizationTemplateInfo orgTmp = new OrganizationTemplateInfo();
                        orgTmp.CreateTime = DateTime.Now;
                        orgTmp.Id = Guid.NewGuid().ToString();
                        orgTmp.TemplateId = item.Id;
                        orgTmp.OrganizationId = organizationId;
                        await _context.OrganizationTemplateInfoes.AddAsync(orgTmp);
                        break;
                    default:
                        break;
                }
                _context.SaveChanges();

                respResult.Message = "OK";
                return respResult;
            }
            catch (Exception ex)
            {
                respResult.IsSuccess = false;
                respResult.Message = string.Format("Something wrong. The exception message:：{0}", ex.Message);
                return respResult;
            }
        }

        [HttpGet]
        public async Task<ResponseResultInfo<List<WordTemplateInfo>>> SearchWordTemplateList(string keyword)
        {
            ResponseResultInfo<List<WordTemplateInfo>> respResult = new ResponseResultInfo<List<WordTemplateInfo>>();
            try
            {

                List<WordTemplateInfo> list = await _context.WordTemplateInfoes.Where(x => x.Type == TemplateType.Public && x.Name.Contains(keyword)).OrderByDescending(x => x.CreateTime).ToListAsync();
                respResult.IsSuccess = true;
                respResult.Result = list;
                return respResult;

            }
            catch (Exception ex)
            {
                //LogHelper.ErrorWriteLine("Something wrong. The exception message:：{0}", ex);
                respResult.IsSuccess = false;
                respResult.Message = string.Format("Something wrong. The exception message:：{0}", ex.Message);
                return respResult;
            }
        }

        [HttpGet]
        public async Task<ResponseResultInfo<List<WordTemplateInfo>>> GetMyPrivateTemplateList(string userId)
        {
            ResponseResultInfo<List<WordTemplateInfo>> respResult = new ResponseResultInfo<List<WordTemplateInfo>>();
            try
            {
                List<string> listIds = _context.PrivateTemplateInfoes.Where(x => x.UserId == userId).OrderByDescending(x => x.CreateTime).Select(x => x.TemplateId).ToList();
                List<WordTemplateInfo> list = await _context.WordTemplateInfoes.Where(x => listIds.Contains(x.Id)).ToListAsync();
                respResult.IsSuccess = true;
                respResult.Result = list;
                return respResult;
            }
            catch (Exception ex)
            {
                respResult.IsSuccess = false;
                respResult.Message = string.Format("Something wrong. The exception message:：{0}", ex.Message);
                return respResult;
            }
        }

        [HttpPost]
        public async Task<ResponseResultInfo> AddMyFavoriteTemplate(string userId, [FromBody]WordTemplateInfo item)
        {
            ResponseResultInfo respResult = new ResponseResultInfo();
            try
            {
                UserFavoriteInfo info = new UserFavoriteInfo();
                info.Id = Guid.NewGuid().ToString();
                info.TemplateId = item.Id;
                info.UserId = userId;
                await _context.UserFavoriteInfoes.AddAsync(info);
                _context.SaveChanges();
                respResult.IsSuccess = true;
                return respResult;
            }
            catch (Exception ex)
            {
                respResult.IsSuccess = false;
                respResult.Message = string.Format("Something wrong. The exception message:：{0}", ex.Message);
                return respResult;
            }
        }

        [HttpGet]
        public async Task<ResponseResultInfo<List<WordTemplateInfo>>> GetMyFavoriteTemplateList(string userId)
        {
            ResponseResultInfo<List<WordTemplateInfo>> respResult = new ResponseResultInfo<List<WordTemplateInfo>>();
            try
            {
                List<string> listIds = _context.UserFavoriteInfoes.Where(x => x.UserId == userId).OrderByDescending(x => x.CreateTime).Select(x => x.TemplateId).ToList();
                List<WordTemplateInfo> list = await _context.WordTemplateInfoes.Where(x => listIds.Contains(x.Id)).ToListAsync();
                respResult.IsSuccess = true;
                respResult.Result = list;
                return respResult;
            }
            catch (Exception ex)
            {
                respResult.IsSuccess = false;
                respResult.Message = string.Format("Something wrong. The exception message:：{0}", ex.Message);
                return respResult;
            }
        }


        [HttpPost]
        public async Task<ResponseResultInfo> AddOrganizationTemplate(string orgId, [FromBody]WordTemplateInfo item)
        {
            ResponseResultInfo respResult = new ResponseResultInfo();
            try
            {
                OrganizationTemplateInfo info = new OrganizationTemplateInfo();
                info.Id = Guid.NewGuid().ToString();
                info.TemplateId = item.Id;
                info.OrganizationId = orgId;
                await _context.OrganizationTemplateInfoes.AddAsync(info);
                _context.SaveChanges();
                respResult.IsSuccess = true;
                return respResult;
            }
            catch (Exception ex)
            {
                respResult.IsSuccess = false;
                respResult.Message = string.Format("Something wrong. The exception message:：{0}", ex.Message);
                return respResult;
            }
        }

        [HttpGet]
        public async Task<ResponseResultInfo<List<WordTemplateInfo>>> GetOrganizationTemplateList(string orgId)
        {
            ResponseResultInfo<List<WordTemplateInfo>> respResult = new ResponseResultInfo<List<WordTemplateInfo>>();
            try
            {
                List<string> listIds = _context.OrganizationTemplateInfoes.Where(x => x.OrganizationId == orgId).OrderByDescending(x => x.CreateTime).Select(x => x.TemplateId).ToList();
                List<WordTemplateInfo> list = await _context.WordTemplateInfoes.Where(x => listIds.Contains(x.Id)).ToListAsync();
                respResult.IsSuccess = true;
                respResult.Result = list;
                return respResult;
            }
            catch (Exception ex)
            {
                respResult.IsSuccess = false;
                respResult.Message = string.Format("Something wrong. The exception message:：{0}", ex.Message);
                return respResult;
            }
        }


        //// GET: api/WordTemplate
        //[HttpGet]
        //public IEnumerable<WordTemplateInfo> GetWordTemplateInfoes()
        //{
        //    return _context.WordTemplateInfoes;
        //}

        //// GET: api/WordTemplate/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetWordTemplateInfo([FromRoute] string id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var wordTemplateInfo = await _context.WordTemplateInfoes.SingleOrDefaultAsync(m => m.Id == id);

        //    if (wordTemplateInfo == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(wordTemplateInfo);
        //}

        //// PUT: api/WordTemplate/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutWordTemplateInfo([FromRoute] string id, [FromBody] WordTemplateInfo wordTemplateInfo)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != wordTemplateInfo.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(wordTemplateInfo).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!WordTemplateInfoExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/WordTemplate
        //[HttpPost]
        //public async Task<IActionResult> PostWordTemplateInfo([FromBody] WordTemplateInfo wordTemplateInfo)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    _context.WordTemplateInfoes.Add(wordTemplateInfo);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetWordTemplateInfo", new { id = wordTemplateInfo.Id }, wordTemplateInfo);
        //}

        //// DELETE: api/WordTemplate/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteWordTemplateInfo([FromRoute] string id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var wordTemplateInfo = await _context.WordTemplateInfoes.SingleOrDefaultAsync(m => m.Id == id);
        //    if (wordTemplateInfo == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.WordTemplateInfoes.Remove(wordTemplateInfo);
        //    await _context.SaveChangesAsync();

        //    return Ok(wordTemplateInfo);
        //}

        //private bool WordTemplateInfoExists(string id)
        //{
        //    return _context.WordTemplateInfoes.Any(e => e.Id == id);
        //}
    }
}